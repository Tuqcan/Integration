using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;

namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Helpers;

/// <summary>
/// orders/stream icin AYRI filtre kurucu.
///
/// ############ NEDEN AYRI BIR SINIF ############
/// Stream'in parametre adlari v2/orders'tan FARKLI ve YANLIS AD SESSIZCE YOK
/// SAYILIYOR (28.08.2026 olcumu):
///     status            -> YOK SAYILIR   (dogrusu: packageItemStatuses)
///     startDate/endDate -> YOK SAYILIR   (dogrusu: lastModifiedStartDate/EndDate)
///     page              -> YOK SAYILIR   (siralamayi nextCursor yurutur)
///
/// Filtrenin uygulanip uygulanmadigi yalnizca nextCursor UZUNLUGUNDAN anlasiliyor
/// (filtresiz 64 -> tek statu 140 -> cift statu 152 karakter). Yani yanlis parametre
/// HATA VERMEZ; akis sessizce magazanin son 3 ayinin TAMAMINI tarar.
///
/// ShipmentFilterBuilder'i stream'e baglamak tam olarak bu hatayi uretirdi.
/// ##############################################
///
/// BILEREK YOK OLANLAR: AddPage, AddStartDate, AddEndDate, AddOrderByField,
/// AddOrderByDirection. Stream'de siralama SABITTIR (lastModifiedDate DESC) ve
/// sayfalamayi cursor yurutur. Bu metotlarin VAR OLMAMASI, yanlis kullanimi
/// DERLEME ZAMANINDA imkansiz kilar.
/// </summary>
public class ShipmentStreamFilterBuilder : IFilterBuilder
{
    /// <summary>
    /// Trendyol DOKUMANI stream icin de "maksimum 2 hafta" diyor.
    /// 28.08.2026 olcumunde KIRPMA GORULMEDI (30 gunluk pencere sonuna kadar akti),
    /// ama belgelenmis bir sinirin bugun uygulanmiyor olmasi yarin da uygulanmayacagi
    /// anlamina GELMEZ; uygulandigi gun v2/orders'takiyle AYNI SESSIZ KAYIP baslar.
    /// </summary>
    public const int MaxWindowDays = 14;

    /// <summary>Olculen tavan: ustu SESSIZCE 200'e kirpiliyor. Varsayilan 50.</summary>
    public const int MaxSize = 200;

    private readonly Dictionary<string, string> _p = new();

    /// <summary>
    /// ZORUNLU. Verilmezse Trendyol pencereyi otomatik olarak SON 2 HAFTA ile sinirlar
    /// (dokuman) -> geri dolum sessizce 2 haftaya iner. Bu yuzden <see cref="Build"/>
    /// eksikligini REDDEDIYOR.
    /// </summary>
    public ShipmentStreamFilterBuilder AddLastModifiedRange(long startMs, long endMs)
    {
        if (endMs <= startMs)
            throw new ArgumentException("endMs > startMs olmali.", nameof(endMs));

        var span = TimeSpan.FromMilliseconds(endMs - startMs);
        if (span > TimeSpan.FromDays(MaxWindowDays))
        {
            throw new ArgumentOutOfRangeException(nameof(endMs),
                $"Pencere {span.TotalDays:F1} gun. Trendyol DOKUMANI stream icin de " +
                $"'maksimum 2 hafta' diyor. 28.08.2026 olcumunde KIRPMA GORULMEDI " +
                $"(30 gunluk pencere sonuna kadar akti) ama belgelenmis bir sinirin bugun " +
                $"uygulanmiyor olmasi yarin da uygulanmayacagi anlamina gelmez; uygulandigi " +
                $"gun v2/orders'takiyle AYNI SESSIZ KAYIP baslar. Pencereyi bolun.");
        }

        _p["lastModifiedStartDate"] = startMs.ToString();
        _p["lastModifiedEndDate"] = endMs.ToString();
        return this;
    }

    /// <summary>
    /// GECERSIZ deger HATA DEGIL, SESSIZ SIFIR uretir:
    /// packageItemStatuses=Zzzz -> HTTP 200, n=0, hasMore=false (olculdu).
    /// Bu yuzden kume <see cref="TrendyolOrderQueryStatuses"/> (Faz 4.1) ile PAYLASILIR
    /// ve burada dogrulanir.
    /// </summary>
    public ShipmentStreamFilterBuilder AddPackageItemStatuses(params PackageStatus[] statuses)
    {
        if (statuses is null || statuses.Length == 0)
            throw new ArgumentException("En az bir statu verilmeli.", nameof(statuses));

        foreach (var st in statuses)
            TrendyolOrderQueryStatuses.Validate(st, nameof(statuses));

        // Virgullu coklu deger CANLI DOGRULANDI: "Delivered,Cancelled" -> ikisi de dondu.
        _p["packageItemStatuses"] = string.Join(",", statuses);
        return this;
    }

    public ShipmentStreamFilterBuilder AddSize(int size)
    {
        if (size < 1 || size > MaxSize)
            throw new ArgumentOutOfRangeException(nameof(size),
                $"1..{MaxSize} olmali. Ustu SESSIZCE {MaxSize}'e kirpiliyor.");

        _p["size"] = size.ToString();
        return this;
    }

    /// <summary>
    /// Cursor OPAKTIR: ayristirilmaz, degistirilmez. Oldugu gibi geri gonderilir.
    /// </summary>
    public ShipmentStreamFilterBuilder AddNextCursor(string cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
            throw new ArgumentException("Bos cursor gonderilmez - akisi bastan baslatir.", nameof(cursor));

        _p["nextCursor"] = cursor;
        return this;
    }

    public string Build()
    {
        if (!_p.ContainsKey("lastModifiedStartDate"))
        {
            throw new InvalidOperationException(
                "lastModifiedStartDate/EndDate ZORUNLU. Verilmezse Trendyol pencereyi " +
                "sessizce SON 2 HAFTA'ya indirir ve geri dolum eksik kalir.");
        }

        // Cursor base64'tur ve '+' / '=' icerebilir -> KACISLANMASI SART,
        // aksi halde '+' sorgu dizesinde bosluga donusur ve cursor bozulur (HTTP 400).
        return string.Join("&", _p.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
    }
}
