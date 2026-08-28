using System.Net;

namespace Integration.Hub;

/// <summary>
/// Pazaryeri ucu 2xx DISINDA bir durum kodu dondurdu (401/403 haric - onlar
/// <see cref="UnauthorizedAccessException"/> olarak firlatilmaya devam eder, cunku
/// ErrorSourceHelper.IsFatalApiError o tipe bakip API anahtarini deaktive ediyor).
///
/// NEDEN AYRI BIR TIP: eskiden bu durum duz <c>new Exception("API istegi basarisiz oldu.
/// StatusCode: 404 ...")</c> olarak firlatiliyordu. Iki somut zarar:
///
/// 1. <b>YANLIS ETIKET.</b> ErrorSourceHelper.Classify duz Exception'i taniyamayip varsayilan
///    dala dusuyordu -> admin ekraninda "Sistem Hatasi" yaziyordu. Yani Trendyol'un 404/500'u
///    BIZIM hatamiz gibi raporlaniyordu ve ekip kendi kodunda hata ariyordu.
///
/// 2. <b>KALICI/GECICI AYRIMI YAPILAMIYORDU.</b> Cagiran taraf durum kodunu goremediginden
///    "bu uc bu kategori icin YOK (404)" ile "Trendyol su an bozuk (500)" ayni muameleyi
///    goruyordu: her ikisi de 3 kez 1 dakika arayla tekrar deneniyordu. Canli olcum
///    (28.08.2026): var olmayan kategoride <c>product/categories/{id}/attributes</c> -> 404.
///    Silinmis bir kategori DB'de kaldigi surece her turda ~2 dakika bosa harcaniyordu.
///
/// Artik cagiran taraf <see cref="IsPermanent"/>'e bakip "tekrar denemenin ANLAMI var mi?"
/// sorusunu yanitlayabiliyor.
/// </summary>
public class MarketplaceApiException : Exception
{
    /// <summary>Pazaryerinin dondurdugu HTTP durum kodu.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>Istek atilan uc (sorgu dizesi dahil).</summary>
    public string Url { get; }

    /// <summary>Yanit govdesi (kirpilmis). Tesis hatalarinda kok nedeni bu tasir.</summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Tekrar denemek ANLAMSIZ mi?
    ///
    /// 4xx = istegin kendisi yanlis ya da hedef yok -> ayni istek yarin da ayni yaniti verir.
    /// TEK ISTISNA 408 (Request Timeout) ve 429 (Too Many Requests): bunlar zamanla degisir.
    /// 5xx = sunucu tarafi, gecici kabul edilir.
    /// </summary>
    public bool IsPermanent =>
        (int)StatusCode >= 400 && (int)StatusCode < 500
        && StatusCode != HttpStatusCode.RequestTimeout
        && (int)StatusCode != 429;

    public MarketplaceApiException(HttpStatusCode statusCode, string url, string? responseBody)
        : base($"Pazaryeri API istegi basarisiz. StatusCode: {(int)statusCode} - {statusCode}. " +
               $"URL: {url}. Response: {Truncate(responseBody)}")
    {
        StatusCode = statusCode;
        Url = url;
        ResponseBody = Truncate(responseBody);
    }

    private static string? Truncate(string? body)
    {
        if (string.IsNullOrEmpty(body)) return body;
        // Yanit govdesi log/e-posta'ya gidiyor; 1 KB tani icin fazlasiyla yeterli,
        // fazlasi BG_CycleLog.ErrorMessage (500) ve mail govdesini sisirir.
        return body.Length <= 1024 ? body : body.Substring(0, 1024) + "...[kirpildi]";
    }
}
