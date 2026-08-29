using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Request;
using Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration;

public class TrendyolPackageIntegration : TrendyolIntegrationBase, ITrendyolPackageIntegration
{
    public TrendyolPackageIntegration(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm, IRateLimiter? rateLimiter = null)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, rateLimiter) { }

    /// <summary>
    /// Siparis paketlerini ceker.
    ///
    /// ############ V1 -> V2 (Faz 3, 28.08.2026) ############
    /// Eski uc: order/sellers/{id}/orders     -> 15.10.2026'da KAPANIYOR.
    /// Yeni uc: order/sellers/{id}/v2/orders
    ///
    /// Canli olcum: V1 ile V2 govdesi BIREBIR AYNI (7.781 byte, content JSON esit).
    /// Fark ZARF'ta degil, SINIRLARDA:
    ///   * V2'de page * size &lt;= 10.000 SERT SINIRI var (asilirsa HTTP 400).
    ///     V1'de bu sinir YOKTU ve bugun HyperCep 10 gunluk pencerede tavanin
    ///     %90'inda. Sayfa dongusunu kesme + pencereyi bolme mantigi
    ///     PackageIntegration.Publisher/Worker.cs icinde (Faz 3.2 / 3.3).
    ///   * 14 gunden genis pencere SESSIZCE kirpiliyor - bkz. ShipmentFilterBuilder.
    /// ######################################################
    /// </summary>
    public async Task<GetShipmentPackagesResponseModel?> GetShipmentPackagesAsync(string filterQuery)
    {
        string url = $"{GetBaseUrl()}order/sellers/{SupplierId}/v2/orders" +
                     (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var response = await GetAsync<GetShipmentPackagesResponseModel>(url, TrendyolRateLimitCategories.ShipmentPackages);
        foreach (var item in response?.Content ?? new List<GetShipmentPackagePackageResponseModel>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }

        return response;
    }

    /// <summary>
    /// Siparis paketlerini AKIS (cursor) ile ceker - TAM TARAMA / GERI DOLUM icin.
    ///
    /// v2/orders'tan farki: 10.000 sayfa tavani YOK, 14 gun kirpmasi YOK.
    /// Veri kapsami AYNI (~3 ay) - stream kapsami GENISLETMEZ (olculdu: 90 gun -> veri,
    /// 100 gun -> 0; v2/orders ile birebir ayni pencere).
    ///
    /// KENDI KOVASINDAN harcar (ShipmentPackagesStream, 12/dk = 5 sn) - artimli
    /// senkronun kovasini yavaslatmasin diye.
    /// </summary>
    public async Task<GetShipmentPackagesStreamResponseModel?> GetShipmentPackagesStreamAsync(string filterQuery)
    {
        string url = $"{GetBaseUrl()}order/sellers/{SupplierId}/orders/stream" +
                     (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var response = await GetAsync<GetShipmentPackagesStreamResponseModel>(
            url, TrendyolRateLimitCategories.ShipmentPackagesStream);

        // v2/orders yolundaki ile AYNI damga: yanittaki supplierId DAIMA 0 (Faz 4.3),
        // magaza kimligi istegi kuran anahtardan gelir.
        foreach (var item in response?.Content ?? new List<GetShipmentPackagePackageResponseModel>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }

        return response;
    }

    // ####################################################################
    // OLU YAZMA UCLARI - PATH'LER DUZELTILDI (Faz 6.5, 28.08.2026)
    //
    // Asagidaki 9 metodun HICBIRININ CAGIRANI YOK. Hepsi eski "suppliers/{id}/..."
    // onekini tasiyordu ve o onek CANLI OLCUMLE 9/9 OLU bulundu (HTTP 556).
    // Yani bu metotlar "muhtemelen yanlis" degildi, BUGUN CALISMIYORLARDI.
    //
    // PATH'LER NASIL DOGRULANDI: GET yerine ucun BEKLEDIGI gercek metot kullanilinca
    // gateway ayrim yapiyor:
    //     400 -> path VAR (govde gecersiz diye reddedildi)
    //     401 -> path YOK (uydurma bir path de 401 donuyor - kontrol satiriyla kanitlandi)
    //     556 -> uc olu
    // Uretim verisi korundu: paket kimligi 1 (gercek kimlikler 2.957.655.730+;
    // 1..1000 araliginda 0 kayit) ve govde {} (gecersiz). Sonuclarin tamami 4xx -
    // hicbir istek islenmedi, hicbir siparis degismedi.
    //
    // NE DOGRULANDI, NE DOGRULANMADI:
    //   Dogrulandi   : path'lerin VARLIGI + HTTP METODU (PUT/POST ayrimi)
    //   DOGRULANMADI : istek/yanit GOVDE SEMASI, alan adlari, zorunluluklar
    // Govde semasi icin gercek bir paket uzerinde YAZMA gerekirdi; bilincli olarak
    // yapilmadi. Bu yuzden hepsi [Obsolete] ve kullanmadan once STAGE'DE test edilmeli.
    // ####################################################################

    /// <summary>
    /// Path CANLI DOGRULANDI (28.08.2026). GOVDE SEMASI DOGRULANMADI.
    /// </summary>
    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> UpdateTrackingNumberAsync(long shipmentPackageId, UpdateTrackingNumberRequestModel updateTrackingNumberRequestModel)
    {
        return await PutAsync<UpdateTrackingNumberRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}/update-tracking-number", updateTrackingNumberRequestModel, TrendyolRateLimitCategories.TrackingNumber);
    }

    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> UpdatePackageAsync(long shipmentPackageId, UpdatePackageRequestModel updatePackageRequestModel)
    {
        return await PutAsync<UpdatePackageRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}", updatePackageRequestModel, TrendyolRateLimitCategories.PackageStatus);
    }

    /// <summary>
    /// ############ KOVA SECIMI BELGELENMEMIS BIR VARSAYIMDIR (Faz 2.2) ############
    /// Bu iki fatura linki ucu PackageStatus kovasindan harciyor, ama Trendyol'un RESMI
    /// limit tablosunda FATURA LINKI SATIRI YOK. Kova secimi olculmus bir gercek degil,
    /// eski koddan devralinan bir TAHMINDIR.
    ///
    /// Bugun etkisiz - cagirani yok. Metot bir gun kullanilirsa kova ONCE dogrulanmali:
    /// yanlis kova, fatura linki trafigini paket statu bildiriminin kotasindan yer ve
    /// iki isi de birbirine yavaslatir.
    /// ###########################################################################
    /// </summary>
    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> SendInvoiceLinkAsync(AddInvoiceLinkRequestModel addInvoiceLinkRequestModel)
    {
        return await PostAsync<AddInvoiceLinkRequestModel, bool>($"{GetBaseUrl()}sellers/{SupplierId}/seller-invoice-links", addInvoiceLinkRequestModel, TrendyolRateLimitCategories.PackageStatus);
    }

    /// <summary>
    /// ############ KOVA SECIMI BELGELENMEMIS BIR VARSAYIMDIR (Faz 2.2) ############
    /// Bu iki fatura linki ucu PackageStatus kovasindan harciyor, ama Trendyol'un RESMI
    /// limit tablosunda FATURA LINKI SATIRI YOK. Kova secimi olculmus bir gercek degil,
    /// eski koddan devralinan bir TAHMINDIR.
    ///
    /// Bugun etkisiz - cagirani yok. Metot bir gun kullanilirsa kova ONCE dogrulanmali:
    /// yanlis kova, fatura linki trafigini paket statu bildiriminin kotasindan yer ve
    /// iki isi de birbirine yavaslatir.
    /// ###########################################################################
    /// </summary>
    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> DeleteInvoiceLinkAsync(DeleteInvoiceLinkRequestModel deleteInvoiceLinkRequestModel)
    {
        return await PostAsync<DeleteInvoiceLinkRequestModel, bool>($"{GetBaseUrl()}sellers/{SupplierId}/seller-invoice-links/delete", deleteInvoiceLinkRequestModel, TrendyolRateLimitCategories.PackageStatus);
    }

    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> SplitMultiPackageByQuantityAsync(long shipmentPackageId, SplitMultiPackageByQuantityRequestModel splitMultiPackageByQuantityRequestModel)
    {
        return await PostAsync<SplitMultiPackageByQuantityRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}/split-packages", splitMultiPackageByQuantityRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> SplitMultiShipmentPackageAsync(long shipmentPackageId, SplitMultiShipmentPackageRequestModel splitMultiShipmentPackageRequestModel)
    {
        return await PostAsync<SplitMultiShipmentPackageRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}/split", splitMultiShipmentPackageRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> SplitShipmentPackageAsync(long shipmentPackageId, SplitShipmentPackageRequestModel splitShipmentPackageRequestModel)
    {
        return await PostAsync<SplitShipmentPackageRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}/multi-split", splitShipmentPackageRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> SplitShipmentPackageByQuantityAsync(long shipmentPackageId, SplitMultiPackageByQuantityRequestModel splitMultiPackageByQuantityRequestModel)
    {
        return await PostAsync<SplitMultiPackageByQuantityRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}/quantity-split", splitMultiPackageByQuantityRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    [Obsolete("Cagirani yok. Path canli dogrulandi (28.08.2026) ama GOVDE SEMASI dogrulanmadi - kullanmadan once stage'de test edin.")]
    public async Task<bool> UpdateBoxInfoAsync(long shipmentPackageId, UpdateBoxInfoRequestModel updateBoxInfoRequestModel)
    {
        return await PutAsync<UpdateBoxInfoRequestModel, bool>($"{GetBaseUrl()}order/sellers/{SupplierId}/shipment-packages/{shipmentPackageId}/box-info", updateBoxInfoRequestModel, TrendyolRateLimitCategories.BoxInfo);
    }
}
