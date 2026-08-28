using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;

namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Helpers;
public class ShipmentFilterBuilder : IFilterBuilder
{
    private readonly Dictionary<string, string> _parameters = new();

    /// <summary>
    /// Trendyol siparis ucunun kabul ettigi EN GENIS tarih penceresi.
    ///
    /// ############ NEDEN SERT BIR SINIR ############
    /// 14 GUNDEN GENIS pencerede uc HATA VERMEZ: HTTP 200 doner, totalElements dolu
    /// gelir, ama endDate'i SESSIZCE startDate + 14 gune ceker - yani EN YENI
    /// siparisler yanitta HIC YOKTUR (28.08.2026 canli olcumu).
    ///
    /// Bugun vurmuyor cunku publisher 10 gunluk pencereler kuruyor. Ama bu davranis
    /// HICBIR YERDE YAZILI DEGILDI: "daha az istek atalim" diye pencereyi 15 gune
    /// cikaran masum bir iyilestirme, siparisleri HATASIZ kaybettirmeye baslardi.
    /// Sinir bu yuzden yoruma degil, KODA gomuldu.
    /// ##############################################
    /// </summary>
    public const int MaxWindowDays = 14;

    private static readonly long MaxWindowMs = (long)TimeSpan.FromDays(MaxWindowDays).TotalMilliseconds;

    /// <summary>
    /// Tarih penceresini TEK PARCADA kurar ve <see cref="MaxWindowDays"/> sinirini zorlar.
    /// Pencere genisse istek KURULMADAN reddedilir - sessiz kirpma yerine yuksek sesle hata.
    /// </summary>
    public ShipmentFilterBuilder AddDateRange(long startDateMs, long endDateMs)
    {
        var spanMs = endDateMs - startDateMs;

        if (spanMs > MaxWindowMs)
        {
            var span = TimeSpan.FromMilliseconds(spanMs);
            throw new ArgumentOutOfRangeException(nameof(endDateMs),
                $"Trendyol {MaxWindowDays} gunden genis pencerede endDate'i SESSIZCE " +
                $"startDate + {MaxWindowDays} gune ceker (28.08.2026 canli olcumu): HTTP 200 doner, " +
                $"totalElements dolu gelir, ama EN YENI siparisler yanitta HIC YOKTUR. " +
                $"Istenen: {span.TotalDays:F1} gun. Pencereyi bolun.");
        }

        _parameters["startDate"] = startDateMs.ToString();
        _parameters["endDate"] = endDateMs.ToString();
        return this;
    }

    /// <summary>
    /// Tek basina cagrildiginda pencere sinirini KACIRIR (bkz. <see cref="MaxWindowDays"/>).
    /// <see cref="Build"/> son bir kontrol yapiyor ama niyet, sinirin ISTEK KURULURKEN
    /// gorulmesi; bu yuzden <see cref="AddDateRange"/> tercih edilir.
    /// </summary>
    [Obsolete("Pencere sinirini kacirir; AddDateRange(startMs, endMs) kullanin.")]
    public ShipmentFilterBuilder AddStartDate(long startDate)
    {
        _parameters["startDate"] = startDate.ToString();
        return this;
    }

    /// <inheritdoc cref="AddStartDate"/>
    [Obsolete("Pencere sinirini kacirir; AddDateRange(startMs, endMs) kullanin.")]
    public ShipmentFilterBuilder AddEndDate(long endDate)
    {
        _parameters["endDate"] = endDate.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddPage(int page)
    {
        _parameters["page"] = page.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddSize(int size)
    {
        if (size > 200)
            throw new Exception("Page size must be less than or equal to 200");
        _parameters["size"] = size.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddSupplierId(long supplierId)
    {
        _parameters["supplierId"] = supplierId.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddOrderNumber(string orderNumber)
    {
        _parameters["orderNumber"] = orderNumber;
        return this;
    }

    /// <summary>
    /// Statu filtresi ekler.
    ///
    /// PackageStatus enum'u API'nin SORGUDA kabul ettigi kumeden GENIS; gecersiz bir
    /// deger HTTP 200 + 0 kayit dondurur (sessiz bos sonuc). Bu yuzden deger ISTEK
    /// KURULMADAN dogrulaniyor - bkz. <see cref="TrendyolOrderQueryStatuses"/>.
    ///
    /// Publisher bugun status KULLANMIYOR (tum statuleri cekiyor); bu koruma ileriye
    /// donuktur: "yalniz Created cekelim, daha hizli olur" diyen bir degisiklik yanlis
    /// enum degeriyle SESSIZCE HICBIR SEY cekmezdi.
    /// </summary>
    public ShipmentFilterBuilder AddStatus(PackageStatus status)
    {
        TrendyolOrderQueryStatuses.Validate(status, nameof(status));
        _parameters["status"] = status.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddOrderByField(OrderField orderByField)
    {
        _parameters["orderByField"] = orderByField.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddOrderByDirection(OrderByDirection orderByDirection)
    {
        _parameters["orderByDirection"] = orderByDirection.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddShipmentPackageIds(List<long> shipmentPackageIds)
    {
        _parameters["shipmentPackageIds"] = string.Join(",", shipmentPackageIds);
        return this;
    }

    /// <summary>
    /// Sorgu dizesini uretir.
    ///
    /// IKINCI KATMAN: AddStartDate/AddEndDate ayri ayri cagrilabildigi icin sinir
    /// tek katmanda kacabilir. Burada son bir kez daha kontrol ediliyor - bir sonraki
    /// kisinin obsolete uyarisini gormezden gelmesi ihtimaline karsi.
    /// </summary>
    public string Build()
    {
        if (_parameters.TryGetValue("startDate", out var rawStart)
            && _parameters.TryGetValue("endDate", out var rawEnd)
            && long.TryParse(rawStart, out var start)
            && long.TryParse(rawEnd, out var end)
            && end - start > MaxWindowMs)
        {
            throw new InvalidOperationException(
                $"Tarih penceresi {MaxWindowDays} gunu asiyor ({TimeSpan.FromMilliseconds(end - start).TotalDays:F1} gun) - " +
                "Trendyol bunu SESSIZCE kirpar ve en yeni siparisler kaybolur. Bkz. AddDateRange.");
        }

        return string.Join("&", _parameters.Select(p => $"{p.Key}={p.Value}"));
    }
}