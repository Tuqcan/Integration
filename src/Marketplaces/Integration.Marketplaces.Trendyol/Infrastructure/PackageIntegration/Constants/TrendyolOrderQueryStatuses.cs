namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;

/// <summary>
/// Trendyol'un SORGU PARAMETRESI olarak kabul ettigi paket statuleri
/// (V2 "status" tablosu, 28.08.2026 - 11 deger).
///
/// ############ NEDEN AYRI BIR KUME ############
/// <see cref="PackageStatus"/> enum'u bundan GENIS (16 deger). Awaiting / Repack /
/// UnDeliveredAndReturned / Verified / Reset yanit GOVDESINDE gorulur (prod DB'de
/// UnDeliveredAndReturned 2.716 satir) ama SORGUDA GECERSIZDIR.
///
/// GECERSIZ DEGER HATA VERMEZ, SESSIZCE 0 KAYIT DONDURUR - iki ucta da olculdu:
///   v2/orders : status=Zzz               -> HTTP 200, totalElements = 0
///   stream    : packageItemStatuses=Zzzz -> HTTP 200, n = 0, hasMore = false
/// Yani bir yazim/enum hatasi "o statude siparis yok" gibi gorunur. Dogrulama bu
/// yuzden ISTEK KURULMADAN once yapilir.
///
/// Kume AYRI ve PUBLIC: iki builder da kullaniyor (v2 "status" ve stream
/// "packageItemStatuses"). Birinin digerinin private alanina erismesi derlenmez.
///
/// ############ DOKUMAN CELISKISI ############
/// Trendyol'un V1 aciklama metni bu listeyi FARKLI veriyor: Repack VAR,
/// AtCollectionPoint / UnPacked YOK. V2 parametre tablosu tam tersini soyluyor.
/// Burada V2 tablosu esas alindi (bkz. plan bolum 1.13 / satir 4).
/// #############################################
/// </summary>
public static class TrendyolOrderQueryStatuses
{
    /// <summary>Sorgu parametresi olarak GECERLI olan 11 statu.</summary>
    public static readonly IReadOnlySet<PackageStatus> Queryable = new HashSet<PackageStatus>
    {
        PackageStatus.Created,
        PackageStatus.Picking,
        PackageStatus.Invoiced,
        PackageStatus.Shipped,
        PackageStatus.Cancelled,
        PackageStatus.Delivered,
        PackageStatus.UnDelivered,
        PackageStatus.Returned,
        PackageStatus.AtCollectionPoint,
        PackageStatus.UnPacked,
        PackageStatus.UnSupplied,
    };

    /// <summary>
    /// Gecersizse aciklayici istisna firlatir. Iki builder da bunu cagirir.
    /// </summary>
    public static void Validate(PackageStatus status, string paramName)
    {
        if (!Queryable.Contains(status))
        {
            throw new ArgumentOutOfRangeException(paramName,
                $"'{status}' Trendyol sorgu parametresi olarak GECERSIZ. Gecersiz deger " +
                $"HTTP 200 + 0 kayit dondurur (sessiz bos sonuc), hata VERMEZ - yani " +
                $"'o statude siparis yok' gibi gorunur. " +
                $"Gecerli degerler: {string.Join(", ", Queryable)}");
        }
    }
}
