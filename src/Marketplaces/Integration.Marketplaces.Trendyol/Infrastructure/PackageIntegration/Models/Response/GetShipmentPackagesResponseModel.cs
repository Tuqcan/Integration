using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;
using System.Text.Json.Serialization;
namespace Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Response;

// #############################################################################
// SOZLESME SERTLESTIRME (Faz 1 - 28.08.2026)
//
// Bu dosyadaki HER alan artik [JsonPropertyName] ile Trendyol'un KANONIK V2 adina
// KILITLI. Oncesinde tek bir attribute yoktu; bag tamamen IntegrationBase'in
// PropertyNameCaseInsensitive davranisina dayaniyordu, yani alan eslesmesi bir
// TESADUFTU. Bunun bedeli olculdu: TcIdentityNumber 433.750 satirin HEPSINDE NULL'di,
// cunku API alani "identityNumber" olarak gonderiyor ve kimse fark etmedi.
//
// Trendyol V1 uclari 15.10.2026'da kapaniyor. Legacy takma adlar (id, amount, price,
// merchantId, sku, ...) o gun kaldirilabilir. Bag artik ACIK oldugu icin kaldirilirsa
// sozlesme testi (OrderContractV2Tests) KIRILIR - uretimde sessizce 0 yazilmaz.
//
// DIKKAT - BU BIR EKLEME DEGIL, BAG DEGISIMIDIR: bir property'ye
// [JsonPropertyName("lineId")] eklendigi anda, PropertyNameCaseInsensitive acik olsa
// bile eski "id" anahtari ARTIK OKUNMAZ (28.08.2026 .NET 8 olcumu). Yani bu dosya
// legacy adlardan kanonik adlara GECISTIR, ikisini birden dinlemek DEGIL.
//
// C# property adlari cogunlukla DEGISMEDI: TY_Package / TY_Package_Line entity'leriyle
// ve TY_PackageProfile konvansiyon eslemesiyle birebir ayni olmalari gerekiyor.
// Korumayi saglayan [JsonPropertyName], C# adi DEGIL.
//
// Plan: developer-md/trendyol-orders-v2-gecis-plani.md Faz 1 + alan eslesme matrisi 2.3
// #############################################################################

public class GetShipmentPackagesResponseModel : PaginationModel
{
    [JsonPropertyName("content")]
    public List<GetShipmentPackagePackageResponseModel> Content { get; set; }
}

public class GetShipmentPackagePackageFastDeliveryOptionResponseModel : IResponseModel
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DeliveryOption Type { get; set; }
}

public class GetShipmentPackagePackageLineResponseModel : IResponseModel
{
    /// <summary>V2 kanonik ad: <c>lineId</c>. Eski ad <c>id</c> idi.</summary>
    [JsonPropertyName("lineId")]
    public long LineId { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("salesCampaignId")]
    public int SalesCampaignId { get; set; }

    /// <summary>Kosullu alan - orneklemde %99 geliyor ama garanti degil.</summary>
    [JsonPropertyName("productSize")]
    public string? ProductSize { get; set; }

    /// <summary>V2 kanonik ad: <c>stockCode</c>. Eski ad <c>merchantSku</c>. DB kolonu YOK.</summary>
    [JsonPropertyName("stockCode")]
    public string MerchantSku { get; set; }

    [JsonPropertyName("productName")]
    public string ProductName { get; set; }

    /// <summary>V2 kanonik ad: <c>contentId</c>. Eski ad <c>productCode</c>.</summary>
    [JsonPropertyName("contentId")]
    public long ProductCode { get; set; }

    /// <summary>Kosullu alan - orneklemde satirlarin yalnizca %37'sinde geliyor.</summary>
    [JsonPropertyName("productOrigin")]
    public string? ProductOrigin { get; set; }

    /// <summary>V2 kanonik ad: <c>sellerId</c>. Eski ad <c>merchantId</c>.</summary>
    [JsonPropertyName("sellerId")]
    public int MerchantId { get; set; }

    /// <summary>V2 kanonik ad: <c>lineGrossAmount</c>. Eski ad <c>amount</c>.</summary>
    [JsonPropertyName("lineGrossAmount")]
    public decimal Amount { get; set; }

    /// <summary>V2 kanonik ad: <c>lineSellerDiscount</c>. Eski ad <c>discount</c>.</summary>
    [JsonPropertyName("lineSellerDiscount")]
    public decimal Discount { get; set; }

    /// <summary>V2 kanonik ad: <c>lineTyDiscount</c>. Eski ad <c>tyDiscount</c>.</summary>
    [JsonPropertyName("lineTyDiscount")]
    public decimal TyDiscount { get; set; }

    [JsonPropertyName("fastDeliveryOptions")]
    public List<GetShipmentPackagePackageFastDeliveryOptionResponseModel> FastDeliveryOptions { get; set; }

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; }

    /// <summary>Kosullu alan - orneklemde %96 geliyor.</summary>
    [JsonPropertyName("productColor")]
    public string? ProductColor { get; set; }

    /// <summary>
    /// DIKKAT: adi TUTAR ima ediyor ama KDV ORANI'dir (20 = %20).
    /// V2 kanonik adi <c>vatRate</c>. Prod DB dogrulamasi (28.08.2026):
    /// 20.00 -> 428.021 satir, 10.00 -> 74.614, 0.00 -> 3.674, 1.00 -> 935.
    /// DB kolon adi (VatBaseAmount) tarihsel kalintidir, DEGISTIRILMEZ
    /// (507.244 satir + tum hesap zinciri).
    /// </summary>
    [JsonPropertyName("vatRate")]
    public decimal VatBaseAmount { get; set; }

    [JsonPropertyName("barcode")]
    public string Barcode { get; set; }

    [JsonPropertyName("orderLineItemStatusName")]
    public string OrderLineItemStatusName { get; set; }

    /// <summary>V2 kanonik ad: <c>lineUnitPrice</c>. Eski ad <c>price</c>.</summary>
    [JsonPropertyName("lineUnitPrice")]
    public decimal Price { get; set; }

    /// <summary>Orneklemde %13 null geliyor - nullable KALMALI.</summary>
    [JsonPropertyName("commission")]
    public decimal? Commission { get; set; }

    /// <summary>
    /// HESAPLANAN ALIAS - JSON'dan OKUNMAZ. "sku" ile "barcode" ayni degeri
    /// tasiyor (prod DB: 507.230/507.244 esit). "sku" legacy takma addir ve V1
    /// kapaninca kaldirilabilir; Sku'yu "sku"ya bagli birakmak TY_Package_Lines.Sku
    /// kolonunun bosalmasina ve kargo desi uyari ekraninin SESSIZCE bosalmasina yol acardi.
    ///
    /// DIKKAT: IKI PROPERTY'YE AYNI [JsonPropertyName("barcode")] VERILEMEZ -
    /// System.Text.Json tip meta verisini kurarken InvalidOperationException atar
    /// ("collides with another property"), yani ILK deserialize'da, deploy sonrasi ILK
    /// siparis cekiminde patlar. Derleme temiz gecer, testler modeli deserialize
    /// etmiyorsa yesil kalir. Bu yuzden alias HESAPLANAN property olarak yazildi
    /// (28.08.2026 .NET 8 olcumu).
    ///
    /// AutoMapper bunu KAYNAK olarak okumaya devam eder (TY_Package_Line.Sku konvansiyonla
    /// eslesir); ReverseMap salt-okunur hedefi zaten atlar.
    /// Kalici cozum Faz 4.5: GetCargoDeciWarningQueries Barcode'a cevrilir, Sku emekliye ayrilir.
    /// </summary>
    [JsonIgnore]
    public string Sku => Barcode;

    // ##### FAZ 6.1 / 6.3 - SATIRDAKI YENI ALANLAR #####

    /// <summary>
    /// Trendyol'un kategori kimligi. Yanitta %100 dolulukla geliyor.
    ///
    /// HANGI KARARI DEGISTIRIYOR: komisyon bugun su zincirle cozuluyor:
    ///     barcode -> TY_Product.PimCategoryId -> TY_User_Categories / TY_Categories
    /// URUN HENUZ SENKRONLANMAMISSA productCache bos kalir ve KOMISYON 0 OLUR.
    /// Bu alan, urun senkronundan BAGIMSIZ bir yedek zincir sagliyor.
    ///
    /// ON KOSUL DOGRULANDI (28.08.2026): kategori uzaylari AYNI.
    /// productCategoryId=5505 -> TY_Categories "Kablo Aksesuari" (%23),
    /// 2710 -> "Tepsi" (%21) - ikisi de siparisin urunuyle TUTARLI.
    /// Katalog geneli: TY_Product.PimCategoryId -> TY_Categories eslesmesi
    /// 203.447/203.447 (%100). Yanlis komisyon riski YOK.
    ///
    /// DB'YE YAZILMAZ - yalnizca komisyon cozumunde yedek olarak okunur.
    /// </summary>
    [JsonPropertyName("productCategoryId")]
    public int? ProductCategoryId { get; set; }

    /// <summary>
    /// Iptali kimin baslattigi.
    ///
    /// HANGI KARARI DEGISTIRIYOR: siparis listesi iptali RENKLENDIRIYOR ama NEDENINI
    /// gosteremiyor. Satici kaynakli iptal (UnSupplied) ile musteri iptali FARKLI
    /// aksiyon gerektirir - biri stok/tedarik sorunu, digeri degil.
    /// </summary>
    /// ############ TIPI DOGRULANMAMISTI - 01.09.2026'DA URETIMDE PATLADI ############
    /// Bu uc alan Faz 6'da <c>string?</c> olarak modellendi. Trendyol
    /// <c>cancelReasonCode</c>'u SAYI gonderiyor ve System.Text.Json bir alanda
    /// patlayinca TUM SAYFAYI birakiyor -> 200 paket birden kayboluyor.
    /// (Deploy sonrasi HyperCep icin 4 turun 4'u dustu, hic siparis yazilmadi.)
    ///
    /// Ucune de <see cref="TolerantStringConverter"/> uygulandi: sayi da metin de
    /// kabul edilir. Ayni ailedeler ve fixture'da UCU DE yoktu (ornek pakette iptal
    /// satiri yok), yani ucunun de tipi ayni sekilde DOGRULANMAMISTI - biri patladiysa
    /// digerlerini "muhtemelen metindir" diye birakmak ayni hatayi tekrar etmek olur.
    /// #############################################################################
    [JsonPropertyName("cancelledBy")]
    [JsonConverter(typeof(TolerantStringConverter))]
    public string? CancelledBy { get; set; }

    /// <inheritdoc cref="CancelledBy"/>
    [JsonPropertyName("cancelReason")]
    [JsonConverter(typeof(TolerantStringConverter))]
    public string? CancelReason { get; set; }

    /// <inheritdoc cref="CancelledBy"/>
    [JsonPropertyName("cancelReasonCode")]
    [JsonConverter(typeof(TolerantStringConverter))]
    public string? CancelReasonCode { get; set; }

    // OKU AMA YAZMA: lineTotalDiscount modellenmedi - Discount + TyDiscount ile
    // TURETILEBILIR. Turetilebilen bir degeri saklamak IKI DOGRULUK KAYNAGI yaratir.
    // businessUnit ve defectiveClaimListingInsight de modellenmedi (bkz. paket modeli).

    // KALDIRILDI (Faz 1.7): DiscountDetails (+ 3 alt alani). 3 repo grep'i ile dogrulandi:
    // is tuketicisi 0 - deserialize edilip atiliyordu. Ileride lazim olursa kanonik
    // adlarla (lineItemSellerDiscount / lineItemTyDiscount / lineItemPrice) geri eklenir.
    // Kullanilmayan bir alani DOGRU tutmak bakim borcudur.
}

public class GetShipmentPackageShipmentAddressResponseModel : IResponseModel
{
    [JsonPropertyName("id")] public long? Id { get; set; }
    [JsonPropertyName("firstName")] public string FirstName { get; set; }
    [JsonPropertyName("lastName")] public string LastName { get; set; }
    [JsonPropertyName("company")] public string Company { get; set; }
    [JsonPropertyName("address1")] public string Address1 { get; set; }
    [JsonPropertyName("address2")] public string Address2 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; }
    [JsonPropertyName("cityCode")] public int CityCode { get; set; }
    [JsonPropertyName("district")] public string District { get; set; }
    [JsonPropertyName("districtId")] public int DistrictId { get; set; }
    [JsonPropertyName("postalCode")] public string PostalCode { get; set; }
    [JsonPropertyName("countryCode")] public string CountryCode { get; set; }
    [JsonPropertyName("neighborhoodId")] public int NeighborhoodId { get; set; }
    [JsonPropertyName("neighborhood")] public string Neighborhood { get; set; }
    [JsonPropertyName("phone")] public object Phone { get; set; }
    [JsonPropertyName("fullName")] public string FullName { get; set; }
    [JsonPropertyName("fullAddress")] public string FullAddress { get; set; }
}

public class GetShipmentPackageInvoiceAddressResponseModel : IResponseModel
{
    [JsonPropertyName("id")] public long? Id { get; set; }
    [JsonPropertyName("firstName")] public string FirstName { get; set; }
    [JsonPropertyName("lastName")] public string LastName { get; set; }
    [JsonPropertyName("company")] public string Company { get; set; }
    [JsonPropertyName("address1")] public string Address1 { get; set; }
    [JsonPropertyName("address2")] public string Address2 { get; set; }
    [JsonPropertyName("city")] public string City { get; set; }
    [JsonPropertyName("district")] public string District { get; set; }
    [JsonPropertyName("districtId")] public int DistrictId { get; set; }
    [JsonPropertyName("postalCode")] public string PostalCode { get; set; }
    [JsonPropertyName("countryCode")] public string CountryCode { get; set; }
    [JsonPropertyName("neighborhoodId")] public int NeighborhoodId { get; set; }
    [JsonPropertyName("neighborhood")] public string Neighborhood { get; set; }
    [JsonPropertyName("phone")] public object Phone { get; set; }
    [JsonPropertyName("fullName")] public string FullName { get; set; }
    [JsonPropertyName("fullAddress")] public string FullAddress { get; set; }
    [JsonPropertyName("taxOffice")] public string TaxOffice { get; set; }
    [JsonPropertyName("taxNumber")] public string TaxNumber { get; set; }
}

public class GetShipmentPackagesPackageHistoryResponseModel : IResponseModel
{
    [JsonPropertyName("createdDate")]
    public long CreatedDate { get; set; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PackageStatus Status { get; set; }
}

public class GetShipmentPackagePackageResponseModel : IResponseModel
{
    /// <summary>
    /// V2 kanonik ad: <c>shipmentPackageId</c>. Eski ad <c>id</c> idi ve bugun de geliyor.
    /// C# adi da DEGISTIRILDI (Id -> ShipmentPackageId): "Id" adi paket ve satir
    /// seviyesinde ayniydi, anlamsizdi ve "p.Id > 0" gibi KRITIK bir filtrenin neyi
    /// suzdugunu gizliyordu. Yeniden adlandirma derleyiciyi gecisin BEKCISI yapar -
    /// etkilenen tum cagri yerleri derleme hatasi verir, hicbiri atlanamaz.
    /// </summary>
    [JsonPropertyName("shipmentPackageId")]
    public long ShipmentPackageId { get; set; }

    [JsonPropertyName("shipmentAddress")]
    public GetShipmentPackageShipmentAddressResponseModel? ShipmentAddress { get; set; }

    [JsonPropertyName("invoiceAddress")]
    public GetShipmentPackageInvoiceAddressResponseModel? InvoiceAddress { get; set; }

    [JsonPropertyName("orderNumber")]
    public string OrderNumber { get; set; }

    /// <summary>V2 kanonik ad: <c>packageGrossAmount</c>. Eski ad <c>grossAmount</c>.</summary>
    [JsonPropertyName("packageGrossAmount")]
    public decimal GrossAmount { get; set; }

    /// <summary>V2 kanonik ad: <c>packageSellerDiscount</c>. Eski ad <c>totalDiscount</c>.</summary>
    [JsonPropertyName("packageSellerDiscount")]
    public decimal TotalDiscount { get; set; }

    /// <summary>V2 kanonik ad: <c>packageTyDiscount</c>. Eski ad <c>totalTyDiscount</c>.</summary>
    [JsonPropertyName("packageTyDiscount")]
    public decimal TotalTyDiscount { get; set; }

    /// <summary>V2 kanonik ad: <c>packageTotalPrice</c>. Eski ad <c>totalPrice</c>.</summary>
    [JsonPropertyName("packageTotalPrice")]
    public decimal TotalPrice { get; set; }

    [JsonPropertyName("taxNumber")]
    public object TaxNumber { get; set; }

    [JsonPropertyName("customerFirstName")]
    public string? CustomerFirstName { get; set; }

    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; set; }

    [JsonPropertyName("customerId")]
    public long CustomerId { get; set; }

    [JsonPropertyName("customerLastName")]
    public string? CustomerLastName { get; set; }

    [JsonPropertyName("cargoTrackingNumber")]
    public long CargoTrackingNumber { get; set; }

    /// <summary>Kosullu alan - orneklemde %60 geliyor.</summary>
    [JsonPropertyName("cargoTrackingLink")]
    public string? CargoTrackingLink { get; set; }

    /// <summary>Kosullu alan - orneklemde %40 geliyor.</summary>
    [JsonPropertyName("cargoSenderNumber")]
    public string? CargoSenderNumber { get; set; }

    [JsonPropertyName("cargoProviderName")]
    public string CargoProviderName { get; set; }

    [JsonPropertyName("lines")]
    public List<GetShipmentPackagePackageLineResponseModel> Lines { get; set; }

    [JsonPropertyName("orderDate")]
    public long OrderDate { get; set; }

    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; }

    [JsonPropertyName("packageHistories")]
    public List<GetShipmentPackagesPackageHistoryResponseModel> PackageHistories { get; set; }

    [JsonPropertyName("shipmentPackageStatus")]
    public string ShipmentPackageStatus { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("deliveryType")]
    public string DeliveryType { get; set; }

    [JsonPropertyName("timeSlotId")]
    public int TimeSlotId { get; set; }

    [JsonPropertyName("estimatedDeliveryStartDate")]
    public long EstimatedDeliveryStartDate { get; set; }

    [JsonPropertyName("estimatedDeliveryEndDate")]
    public long EstimatedDeliveryEndDate { get; set; }

    [JsonPropertyName("deliveryAddressType")]
    public string DeliveryAddressType { get; set; }

    [JsonPropertyName("agreedDeliveryDate")]
    public long AgreedDeliveryDate { get; set; }

    /// <summary>Kosullu alan - orneklemde %81 geliyor.</summary>
    [JsonPropertyName("invoiceLink")]
    public string? InvoiceLink { get; set; }

    [JsonPropertyName("fastDelivery")]
    public bool FastDelivery { get; set; }

    [JsonPropertyName("fastDeliveryType")]
    public string? FastDeliveryType { get; set; }

    [JsonPropertyName("originShipmentDate")]
    public long OriginShipmentDate { get; set; }

    [JsonPropertyName("lastModifiedDate")]
    public long LastModifiedDate { get; set; }

    [JsonPropertyName("commercial")]
    public bool Commercial { get; set; }

    [JsonPropertyName("deliveredByService")]
    public bool DeliveredByService { get; set; }

    [JsonPropertyName("micro")]
    public bool Micro { get; set; }

    /// <summary>
    /// Mikro ihracat siparislerinde DUZENLI olarak dolu gelir - "orneklemde gormedim"
    /// ile "gelmiyor" ayni sey DEGIL. Prod DB kaniti (28.08.2026): 880567 -> 1.297 dolu,
    /// 1169190 -> 496, 193500 -> 370. SILINMEZ; yalnizca nullable yapildi.
    /// </summary>
    [JsonPropertyName("etgbNo")]
    public string? EtgbNo { get; set; }

    /// <inheritdoc cref="EtgbNo"/>
    [JsonPropertyName("etgbDate")]
    public long? EtgbDate { get; set; }

    [JsonPropertyName("giftBoxRequested")]
    public bool GiftBoxRequested { get; set; }

    [JsonPropertyName("3pByTrendyol")]
    public bool? Is3pByTrendyol { get; set; }

    [JsonPropertyName("containsDangerousProduct")]
    public bool ContainsDangerousProduct { get; set; }

    // ##### FAZ 6.1 - DB KOLONU HAK EDEN YENI ALANLAR #####
    // Olcut TEK: bir alan ancak HANGI KARARI DEGISTIRDIGI yazilabiliyorsa kolon hak eder.
    // Cevap yoksa modelde okunur (bedava), DB'ye yazilmaz (bkz. asagidaki "OKU AMA YAZMA").

    /// <summary>
    /// Satis kanali. 1 = CORE, 25 = Luxe.
    ///
    /// HANGI KARARI DEGISTIRIYOR: Luxe'un komisyon ve hakedis rejimi FARKLIDIR; kar
    /// hesabi bugun ikisini ayirt etmiyor. Luxe satan ilk kullanicida SESSIZCE YANLIS
    /// KAR uretir.
    ///
    /// ⛔ ENUM'A CEVIRME. Olcumde dokumanda OLMAYAN bir deger de gorüldu: 9 (5 paket).
    /// Bilinmeyen bir kanali "Unkown" enum'una dusurmek, PackageStatus'te yasanan
    /// tuzagin aynisini uretirdi: ham deger kaybolur ve geriye donuk cozulemez.
    /// Ham INT saklanir, etiket UI'da cozulur.
    /// </summary>
    [JsonPropertyName("channelId")]
    public int? ChannelId { get; set; }

    /// <summary>
    /// Pazaryerinin KENDI desisi.
    ///
    /// HANGI KARARI DEGISTIRIYOR: GetCargoDeciWarning ekrani desiyi bugun
    /// TY_Invoice_Cargo'dan (FATURA) aliyor - yani fatura kesilene kadar uyari YOK.
    /// API desiyi siparisle BIRLIKTE veriyor -> FATURA ONCESI desi sapmasi yakalanir.
    /// </summary>
    [JsonPropertyName("cargoDeci")]
    public decimal? CargoDeci { get; set; }

    /// <summary>
    /// Paketin nasil olustugu: order-creation / split / cancel / transfer.
    ///
    /// HANGI KARARI DEGISTIRIYOR: createdBy='cancel' olan paket, iptal edilen bir
    /// paketin KALANIDIR. <see cref="OriginPackageIds"/> aslini gosterir. Kismi iade
    /// muhasebesinde bu bag BUGUN YOK.
    /// </summary>
    [JsonPropertyName("createdBy")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Bolunme/iptal oncesi paket kimlikleri. Yalniz createdBy=cancel paketlerde dolu.
    /// <inheritdoc cref="CreatedBy"/>
    /// </summary>
    [JsonPropertyName("originPackageIds")]
    public List<long>? OriginPackageIds { get; set; }

    // ##### OKU AMA DB'YE YAZMA #####
    // Asagidaki alanlar modelde OKUNUR (bedava) ama DB'ye YAZILMAZ - "hangi karari
    // degistiriyor" sorusunun bugun bir cevabi yok:
    //   invoiceStatus / invoiceNumber : GelirUP fatura KESMIYOR; invoiceNumber ayrica
    //                                   1.322 pakette %0 dolu - bos alani saklamak yaniltici
    //   warehouseId                   : coklu depo raporu talebi yok
    //   is4P / hsCode                 : "micro" zaten var, kullanici yok
    //   businessUnit                  : Trendyol'un IC siniflandirmasi; PimCategoryId var
    //   discountDisplays              : kampanya adi kirilimi; toplam TotalDiscount'ta
    //   sellerDeliveryMethod / sellerOtpCode / taxNumber : TR pazaryerinde DAIMA bos
    //   supplierId                    : DAIMA 0 (Faz 4.3)
    // Modellenmiyorlar cunku okunmayan bir alani DOGRU tutmak da bakim borcudur.

    // ##### API'DEN GELMEYEN, KODUN KENDI DAMGALADIGI ALANLAR #####
    // Bu ikisi Trendyol yanitinda YOKTUR; publisher tarafindan atanir ve mesaj
    // RabbitMQ uzerinden consumer'a tasinirken serilestirilir.
    //
    // BUNLARA [JsonPropertyName] EKLEME. Adlarini MassTransit'in adlandirma politikasi
    // belirliyor; attribute eklemek TEL FORMATINI DEGISTIRIR ve TY_SUPPLIERID'yi 0'a
    // dusurur -> tum paketler "supplier 0" altinda gruplanir, YANLIS maliyet/komisyon
    // cekilir.
    //
    // ⛔ YANITTAKI "supplierId" ALANI MODELLENMEDI - BILINCLI (Faz 4.3).
    // Trendyol o alani DAIMA 0 gonderiyor (28.08.2026, 1.322 pakette istisnasiz).
    // Modellenseydi birisi "hazir geliyormus" diye kullanir ve tum paketleri
    // supplier 0'a yazardi. Yaniltici bir alani DOGRU tutmaktansa HIC bulundurmamak
    // yegdir. Magaza kimligi asagidaki TY_SUPPLIERID'den gelir; onu
    // TrendyolPackageIntegration istegi kuran anahtardan ELLE atar - dogru davranis.

    public int TY_SUPPLIERID { get; set; }

    /// <summary>
    /// Senkronu tetikleyen kullanicinin UserId'si. Publisher (PackagePublish) damgalar.
    /// Consumer cost/komisyonu bu kullaniciya gore ceker (cok-kullanicili magaza icin).
    /// </summary>
    public Guid UserId { get; set; }

    // ##### KALDIRILAN ALANLAR (Faz 1.4 / 1.5) #####
    //
    // TcIdentityNumber - KALDIRILDI (28.08.2026 kullanici karari: "gerek yok").
    //   Trendyol bu alani "identityNumber" olarak gonderiyor, model "tcIdentityNumber"
    //   ariyordu -> 433.750/433.750 satir NULL. Bag ONARILMADI: alan TCKN tasiyor
    //   (altin, gubre ve 5.000 TL ustu siparislerde dolu gelir), kod tabaninda tek bir
    //   IS TUKETICISI YOK ve saklamak KVKK sorumlulugu dogurur.
    //   TY_Package.TcIdentityNumber kolonu OLU olarak birakildi (silinmedi: 433.857
    //   satirlik tabloda migration + risk demek, kazanci yok).
    //
    // ScheduledDeliveryStoreId - KALDIRILDI. Yanitta HIC gelmiyor (1.322 pakette 0 kez)
    //   ve TY_Package entity'sinde de tabloda da KOLONU YOK; deserialize edilip atiliyordu.
    //
    // AgreedDeliveryDateExtendible / ExtendedAgreedDeliveryDate /
    // AgreedDeliveryExtensionStartDate / AgreedDeliveryExtensionEndDate - KALDIRILDI.
    //   Yanitta HIC gelmiyor. DB kaniti: GROUP BY ile TEK satir -> 0 | 0 | 0 | 433.857,
    //   yani istisnasiz hepsi 0. Kolonlar KALIYOR (433.857 satirda migration riski,
    //   kazanci yok) ama TY_PackageProfile'da opt.Ignore() ile isaretlendi ve entity
    //   XML doc'una "OLU KOLON - OKUMAYIN" notu dusuldu.
}
