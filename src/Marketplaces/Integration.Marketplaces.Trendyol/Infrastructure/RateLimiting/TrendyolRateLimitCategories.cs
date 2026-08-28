namespace Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

public static class TrendyolRateLimitCategories
{
    // =====================================================================
    // URUN SERVISLERI - 14.09.2026 ORTAK KOVA REJIMI
    //
    // Trendyol bu tarihten itibaren urun uclarini TEK TEK degil, GRUP olarak
    // limitliyor: tum urun SORGU uclari ayni kovadan, tum urun YAZMA uclari
    // ayni kovadan harcaniyor. Limit degeri saticinin listeleme kotasi
    // tier'ine bagli (bkz. TrendyolRateLimitConfig).
    //
    // Kova kimligi = SABITIN DEGERI. RedisRateLimiter Redis anahtarini
    // "...:ratelimit:{deger}" olarak kuruyor; asagidaki takma adlarin hepsi
    // ayni degere isaret ettigi icin GERCEKTEN tek kovadan geciyorlar.
    // Cagiran kod hic degismedi - okunabilirlik icin isimler korundu.
    // =====================================================================

    /// <summary>
    /// SATICIYA AIT urun SORGU uclarinin ortak kovasi (@T50K 1000/dk).
    ///
    /// Kova SATICI BAZLI anahtarlanir (bkz. IntegrationBase.ApplyRateLimitAsync):
    /// limit saticinin kendi listeleme kotasi tier'inden turedigi icin dogrusu budur.
    /// Global anahtarlanirsa N magaza limitin 1/N'ine duser ve 500K tier'lik satici
    /// 50K tier'lik saticiyla ayni hizda calisir - tier okumanin anlami kalmaz.
    /// </summary>
    public const string ProductRead = "ProductRead";

    /// <summary>Urun olusturma/guncelleme/silme ortak kovasi (@T50K 200/dk). Satici bazli.</summary>
    public const string ProductWrite = "ProductWrite";

    /// <summary>Stok & fiyat guncelleme kovasi (@T50K 350/dk). Eskiden LIMITSIZ sayiliyordu.</summary>
    public const string StockPriceWrite = "StockPriceWrite";

    /// <summary>
    /// ORTAK KATALOG uclari: kategori agaci, kategori-ozellik, ozellik DEGERLERI, markalar.
    ///
    /// ############ NEDEN ProductRead'DEN AYRI ############
    /// Bu uclar SATICIYA OZGU DEGIL - Trendyol'un genel kataloguna aittir ve kimlik
    /// bilgisi bile istemiyorlar. Canli olcum (28.08.2026, ANONIM istek):
    ///   product/product-categories                    -> 200
    ///   product/categories/766/attributes             -> 200
    ///   product/categories/766/attributes/348/values  -> 200
    ///   70 istek / 10,4 saniye (~400 istek/dk)        -> 429 YOK
    ///
    /// Yani bu istekler hicbir saticinin urun-okuma kotasina YAZILAMAZ. ProductRead ile
    /// ayni kovaya konulursa CategoriesWorker'in tur basina ~3.400 katalog istegi
    /// dogrudan ProductWorker'in satici kotasini yer - hicbir karsiligi olmayan bir aclik.
    ///
    /// Limit yine de sonsuz DEGIL: kendimizi (ve Trendyol'u) korumak icin makul bir tavan
    /// var (bkz. TrendyolRateLimitConfig). Kova saticidan bagimsiz oldugu icin GLOBAL
    /// anahtarlanir - katalog islerinde supplierId zaten bostur.
    /// ###################################################
    /// </summary>
    public const string CatalogRead = "CatalogRead";

    // --- Satici bazli urun okuma takma adlari (ProductRead kovasindan harcar) ---
    public const string ProductFilter = ProductRead;
    public const string BatchCheck = ProductRead;

    // --- Katalog takma adlari (CatalogRead kovasindan harcar) ---
    public const string Brands = CatalogRead;
    public const string Categories = CatalogRead;
    public const string CategoryAttributes = CatalogRead;
    public const string CategoryAttributeValues = CatalogRead;

    // --- Yazma takma adlari (hepsi ProductWrite kovasindan harcar) ---
    public const string ProductCreate = ProductWrite;
    public const string ProductUpdate = ProductWrite;
    public const string ProductDelete = ProductWrite;

    /// <summary>
    /// Tedarikci adresleri. Urun grubunda DEGIL (siparis servisleri altinda) ve
    /// kendi sert limiti var: 1/saat. Ortak kovaya KATILMAZ - katilsaydi tek bir
    /// adres cagrisi saatlerce tum urun okumalarini kilitlerdi.
    /// </summary>
    public const string SupplierAddresses = "SupplierAddresses";       // 1/hour

    // Siparis Servisleri (Limit 50000 tier)
    public const string ShipmentPackages = "ShipmentPackages";         // 30/min (8 Haz 2026, 50000 tier)
    public const string TrackingNumber = "TrackingNumber";             // 300/min
    public const string PackageStatus = "PackageStatus";               // 300/min
    public const string SplitPackages = "SplitPackages";               // 100/min
    public const string BoxInfo = "BoxInfo";                           // 100/min

    // Iade Servisleri
    public const string ClaimsList = "ClaimsList";                     // 1000/min
    public const string ClaimApprove = "ClaimApprove";                 // 5/min

    // Muhasebe ve Finans Servisleri
    public const string InvoiceSettlements = "InvoiceSettlements";     // 100/min
    public const string InvoiceCargo = "InvoiceCargo";                 // 100/min

    // QnA (Müşteri Soruları) Servisleri
    public const string QnAFilter = "QnAFilter";                      // 1000/min
    public const string QnAAnswer = "QnAAnswer";                      // 500/min
}
