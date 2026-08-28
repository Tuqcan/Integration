using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Integration.Hub;
public abstract class IntegrationBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    protected readonly string SupplierId;
    protected readonly string ApiKey;
    protected readonly string ApiSecret;
    protected readonly JsonSerializerOptions _jsonOptions;
    protected readonly IRateLimiter? _rateLimiter;

    private const int MaxTransientRetries = 3;

    /// <summary>
    /// 429 (Too Many Requests) icin azami deneme sayisi.
    ///
    /// Eskiden 429 dali "while(true)" icinde SINIRSIZ donuyordu ve
    /// <see cref="MaxTransientRetries"/> sayaci 429'da ARTMIYORDU: limit kapanmazsa
    /// worker sonsuza kadar asili kalir, heartbeat "calisiyor" demeye devam ederdi.
    /// Artik hak bitince <see cref="RateLimitExceededException"/> firlatiliyor.
    /// </summary>
    private const int MaxRateLimitRetries = 5;

    /// <summary>
    /// Tek bir 429 beklemesinin ust siniri.
    ///
    /// Retry-After basligi cok buyuk bir deger sallarsa (ya da uc bozuk bir deger
    /// donerse) tek bir istek saatlerce uyuyabilirdi - kacinmaya calistigimiz
    /// "sessiz aski" durumunun ta kendisi. Sinir yuzunden erken uyanip yeniden
    /// 429 alirsak bir deneme hakki harcanir ve en fazla MaxRateLimitRetries
    /// sonunda YUKSEK SESLE hata aliriz; sessizce asili kalmaktan iyidir.
    /// </summary>
    private static readonly TimeSpan MaxRateLimitDelay = TimeSpan.FromMinutes(5);

    /// <summary>Retry-After yoksa ya da anlamsizsa kullanilan varsayilan bekleme.</summary>
    private static readonly TimeSpan DefaultRateLimitDelay = TimeSpan.FromSeconds(60);

    /// <summary>
    /// HTTP 426 Upgrade Required. <see cref="HttpStatusCode"/>'da .NET 8'de karsiligi YOK,
    /// bu yuzden sabit olarak tanimlandi (her yerde "(HttpStatusCode)426" yazmamak icin).
    /// </summary>
    private const HttpStatusCode UpgradeRequired = (HttpStatusCode)426;

    /// <summary>
    /// 426 icin AYRI deneme butcesi.
    ///
    /// Trendyol V1 uclarini 15.10.2026'ya kadar gunde 3 kez 10'AR DAKIKA kapatiyor.
    /// Genel gecici-hata butcesi (<see cref="MaxTransientRetries"/> = 3, ustel geri
    /// cekilme ile toplam ~14 saniye) bu pencereyi karsilamaz. Genel sayaci buyutmek
    /// YANLIS cozum olurdu: o zaman GERCEK bir kesintide (500/503) worker dakikalarca
    /// asili kalirdi. Bu yuzden 426'nin kendi sayaci var.
    ///
    /// 12 deneme x 60 saniye = ~12 dakika, yani 10 dakikalik pencereyi paylı karsilar.
    /// Hak bitince istek 426 ile geri doner ve normal hata yolu isler - sessiz aski YOK.
    /// </summary>
    private const int MaxUpgradeRequiredRetries = 12;

    protected IntegrationBase(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, IRateLimiter? rateLimiter = null)
    {
        _httpClientFactory = httpClientFactory;
        SupplierId = supplierId;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        _rateLimiter = rateLimiter;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    protected HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("TrendyolApi");
        var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ApiKey}:{ApiSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        AddHeaders(client);

        return client;
    }

    protected virtual void AddHeaders(HttpClient client) { }

    #region Rate limit

    /// <summary>
    /// Rate limit kovasini uygular.
    ///
    /// ############ KOVA SATICI BAZLI ############
    /// <see cref="SupplierId"/> pozitif bir sayiysa SATICI BAZLI kova kullanilir
    /// (<c>...:ratelimit:{kategori}:{supplierId}</c>); aksi halde GLOBAL kova
    /// (<c>...:ratelimit:{kategori}</c>).
    ///
    /// NEDEN: Trendyol'un yayinladigi limitler SATICININ kendi kotasina baglidir -
    /// "tier" kavraminin tamami saticinin listeleme kotasindan turer
    /// (T50K/T75K/.../Unlimited). Limit satici bazli oldugu halde kovayi global
    /// tutmak iki yonlu zarar veriyordu:
    ///   * N magaza tek kovayi paylasinca her magaza limitin 1/N'ine dusuyordu
    ///     (kendi kotasini KULLANAMIYORDU) - SaaS'ta magaza sayisi arttikca
    ///     her magaza lineer olarak yavasliyor.
    ///   * 500K tier'lik bir satici ile 50K tier'lik bir satici ayni hizda
    ///     calisiyordu; tier okumanin bir anlami kalmiyordu.
    ///
    /// KATALOG UCLARI (kategori agaci, kategori-ozellik, ozellik degerleri, markalar)
    /// saticiya OZGU DEGILDIR ve zaten kimlik bilgisi olmadan cagriliyor
    /// (CategoriesWorker supplierId'yi bos geciyor) -> otomatik olarak global kovaya
    /// duserler. Bu dogru davranis: o istekler hicbir saticinin kotasina yazilamaz.
    /// ###########################################
    /// </summary>
    private async Task ApplyRateLimitAsync(string? rateLimitCategory, CancellationToken ct)
    {
        if (rateLimitCategory is null || _rateLimiter is null)
            return;

        if (int.TryParse(SupplierId, out var supplierId) && supplierId > 0)
            await _rateLimiter.WaitAsync(rateLimitCategory, supplierId, ct);
        else
            await _rateLimiter.WaitAsync(rateLimitCategory, ct);
    }

    /// <summary>
    /// 429 sonrasi beklenecek sureyi hesaplar; deneme hakki bittiyse hata firlatir.
    /// TUM HTTP metotlari bu tek noktadan gecer (kural kopyalarda sapmasin).
    /// </summary>
    private static TimeSpan NextRateLimitDelay(HttpResponseMessage response, string url, int attempt)
    {
        if (attempt > MaxRateLimitRetries)
            throw new RateLimitExceededException(url, MaxRateLimitRetries);

        var retryAfter = ReadRetryAfter(response) ?? DefaultRateLimitDelay;

        if (retryAfter > MaxRateLimitDelay) return MaxRateLimitDelay;
        // Negatif/sifir gelirse hic beklemeden yeniden denemek 429 firtinasi uretir.
        if (retryAfter < TimeSpan.FromSeconds(1)) return TimeSpan.FromSeconds(1);
        return retryAfter;
    }

    /// <summary>
    /// Retry-After basligi IKI bicimde gelebilir (RFC 9110): saniye cinsinden "delta"
    /// ya da HTTP-date. Eskiden yalnizca <c>Delta</c> okunuyordu; tarih bicimi gelirse
    /// null kalip 60 saniyeye dusuyordu - yani pazaryerinin soyledigi sure GOZ ARDI
    /// ediliyordu. Ikisi de destekleniyor.
    /// </summary>
    private static TimeSpan? ReadRetryAfter(HttpResponseMessage response)
    {
        var header = response.Headers.RetryAfter;
        if (header is null) return null;
        if (header.Delta.HasValue) return header.Delta.Value;
        if (header.Date.HasValue) return header.Date.Value - DateTimeOffset.UtcNow;
        return null;
    }

    #endregion

    #region Rate-Limited HTTP Methods

    /// <summary>
    /// TEK GONDERIM CEKIRDEGI.
    ///
    /// Eskiden Get/Post/Put/Delete (+govdeli Delete) metotlarinin her biri ayni retry
    /// dongusunun KENDI KOPYASINI tasiyordu. Bes kopya bes kez sapabilir: 429 sayacinin
    /// yalnizca bazi metotlarda artmamasi tam olarak bu yuzden olusmustu. Politika
    /// (rate limit -> gonder -> soket hatasi retry -> 429 tavani -> gecici durum retry)
    /// artik TEK yerde.
    ///
    /// <paramref name="requestFactory"/> her denemede YENIDEN cagrilir:
    /// <see cref="HttpRequestMessage"/> tek kullanimliktir, ikinci denemede ayni nesne
    /// gonderilirse "request already sent" hatasi alinir.
    /// </summary>
    private async Task<HttpResponseMessage> SendWithPolicyAsync(
        Func<HttpRequestMessage> requestFactory,
        string url,
        string? rateLimitCategory,
        CancellationToken ct)
    {
        int transientRetries = 0;
        int rateLimitRetries = 0;
        int upgradeRequiredRetries = 0;

        while (true)
        {
            ct.ThrowIfCancellationRequested();

            await ApplyRateLimitAsync(rateLimitCategory, ct);

            using var client = CreateClient();
            using var request = requestFactory();

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            }
            catch (HttpRequestException) when (transientRetries < MaxTransientRetries && !ct.IsCancellationRequested)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)), ct);
                continue;
            }

            if ((int)response.StatusCode == 429)
            {
                // Sayac 429'da da ARTAR (eskiden artmiyordu -> sinirsiz dongu).
                var delay = NextRateLimitDelay(response, url, ++rateLimitRetries);
                response.Dispose();
                await Task.Delay(delay, ct);
                continue;
            }

            // 426 GENEL gecici-hata dalindan ONCE ele alinir: kendi butcesi ve kendi
            // bekleme suresi var (bkz. MaxUpgradeRequiredRetries). Ustel geri cekilme
            // KULLANILMAZ - bakim penceresi sabit uzunlukta (10 dk), duzenli araliklarla
            // yoklamak dogru davranis. Trendyol Retry-After gonderirse ona uyulur.
            if (response.StatusCode == UpgradeRequired && upgradeRequiredRetries < MaxUpgradeRequiredRetries)
            {
                upgradeRequiredRetries++;
                var upgradeDelay = ReadRetryAfter(response) ?? DefaultRateLimitDelay;
                if (upgradeDelay > MaxRateLimitDelay) upgradeDelay = MaxRateLimitDelay;
                if (upgradeDelay < TimeSpan.FromSeconds(1)) upgradeDelay = TimeSpan.FromSeconds(1);
                response.Dispose();
                await Task.Delay(upgradeDelay, ct);
                continue;
            }

            if (IsTransientError(response.StatusCode) && transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                response.Dispose();
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)), ct);
                continue;
            }

            return response;
        }
    }

    protected async Task<TResponse> GetAsync<TResponse>(string url, string? rateLimitCategory = null, CancellationToken ct = default)
    {
        using var response = await SendWithPolicyAsync(
            () => new HttpRequestMessage(HttpMethod.Get, url), url, rateLimitCategory, ct);

        return await HandleResponse<TResponse>(response, url);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request, string? rateLimitCategory = null, CancellationToken ct = default)
    {
        using var response = await SendWithPolicyAsync(
            () => BuildJsonRequest(HttpMethod.Post, url, request), url, rateLimitCategory, ct);

        return await HandleResponse<TResponse>(response, url);
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest request, string? rateLimitCategory = null, CancellationToken ct = default)
    {
        using var response = await SendWithPolicyAsync(
            () => BuildJsonRequest(HttpMethod.Put, url, request), url, rateLimitCategory, ct);

        return await HandleResponse<TResponse>(response, url);
    }

    /// <summary>
    /// Govdesiz DELETE. DIKKAT: bu overload HATA FIRLATMAZ, yalnizca basari bayragi
    /// doner (tarihsel sozlesme - cagiranlar bool bekliyor).
    /// </summary>
    protected async Task<bool> DeleteAsync(string url, string? rateLimitCategory = null, CancellationToken ct = default)
    {
        using var response = await SendWithPolicyAsync(
            () => new HttpRequestMessage(HttpMethod.Delete, url), url, rateLimitCategory, ct);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// GOVDELI DELETE.
    ///
    /// <see cref="HttpClient.DeleteAsync(string)"/> govde tasiyamaz; bu yuzden istek
    /// elle <see cref="HttpRequestMessage"/> ile kuruluyor. Trendyol V2 urun silme ucu
    /// (product/sellers/{id}/products) DELETE metodu + JSON govde bekliyor - eski kod
    /// PUT kullaniyordu.
    /// </summary>
    protected async Task<TResponse> DeleteAsync<TRequest, TResponse>(string url, TRequest request, string? rateLimitCategory = null, CancellationToken ct = default)
    {
        using var response = await SendWithPolicyAsync(
            () => BuildJsonRequest(HttpMethod.Delete, url, request), url, rateLimitCategory, ct);

        return await HandleResponse<TResponse>(response, url);
    }

    private HttpRequestMessage BuildJsonRequest<TRequest>(HttpMethod method, string url, TRequest request)
    {
        var jsonData = JsonSerializer.Serialize(request, _jsonOptions);
        return new HttpRequestMessage(method, url)
        {
            Content = new StringContent(jsonData, Encoding.UTF8, "application/json")
        };
    }

    #endregion

    private static bool IsTransientError(HttpStatusCode code)
        => code is HttpStatusCode.InternalServerError
            or HttpStatusCode.BadGateway
            or HttpStatusCode.ServiceUnavailable
            or HttpStatusCode.GatewayTimeout
            // 426 Upgrade Required: Trendyol V1 uclarina 15.10.2026'ya kadar GUNDE 3 KEZ
            // 10'ar dakika BILEREK donduruluyor (30.07.2026 duyurusu). Planli bir bakim
            // penceresidir, ARIZA DEGIL. Genel Exception'a dusseydi RetryHelper 3 denemede
            // pes edip admin'e e-posta atar ve BG_CycleLog'a "Failed" yazardi: beklenen bir
            // bakim penceresi GERCEK ARIZA gibi raporlanirdi.
            //
            // NOT: asil bekleme SendWithPolicyAsync'teki OZEL 426 dalinda yapilir - genel
            // gecici-hata butcesi (MaxTransientRetries=3 -> 2+4+8 = 14 saniye) 10 dakikalik
            // pencereyi KARSILAMAZ. Burada da geciciye sayilmasi, ozel dalin hakki
            // bittikten sonra son birkac denemenin yine de yapilmasini saglar.
            // 15.10 sonrasi bu dal olur ama zararsizdir.
            or UpgradeRequired;

    private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response, string url)
    {
        try
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException($"Trendyol API erisim hatasi. StatusCode: {(int)response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                // Tiplendirilmis: cagiran taraf durum koduna bakip KALICI/GECICI ayrimi
                // yapabiliyor ve ErrorSourceHelper "Sistem Hatasi" yerine dogru etiketi
                // basiyor. Bkz. MarketplaceApiException.
                throw new MarketplaceApiException(response.StatusCode, url, errorContent);
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TResponse>(responseStream, _jsonOptions)
                   ?? throw new JsonException("JSON donusumu basarisiz.");
        }
        catch (OutOfMemoryException ex)
        {
            throw new OutOfMemoryException("Bellek asimi olustu! JSON verisi cok buyuk olabilir.", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException("JSON hatasi: Gecersiz format!", ex);
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException
                                      and not OutOfMemoryException
                                      and not JsonException
                                      and not MarketplaceApiException)
        {
            // MarketplaceApiException BU DALA DUSMEMELI: sarilirsa durum kodu kaybolur
            // ve Classify/IsPermanent yeniden korlesir - duzeltmenin tum amaci buydu.
            throw new Exception($"Istek islenirken hata olustu: {ex.Message}", ex);
        }
    }
}
