namespace Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

public record RateLimitRule(int Limit, TimeSpan Window);

/// <summary>
/// Trendyol'un yayinladigi resmi limitler.
/// Kaynak: developers.trendyol.com/docs/1-servis-limitleri
///
/// 14.09.2026 REJIM DEGISIKLIGI: urun uclari artik TEK TEK degil GRUP olarak
/// limitleniyor ve limit degeri saticinin listeleme kotasi tier'ine bagli.
/// Eski konfig her uca AYRI kova veriyordu (ornegin ProductFilter tek basina
/// 2000/dk) - yani T50K tavaninin (1000/dk) IKI KATINA izin veriyordu. Bu haliyle
/// 14 Eylul sonrasi 429 kacinilmazdi.
/// </summary>
public static class TrendyolRateLimitConfig
{
    /// <summary>
    /// Tier okunamadiginda/gecersiz oldugunda kullanilan deger.
    ///
    /// NEDEN EN DAR TIER: hatanin maliyeti asimetrik. Genis varsayip dar tier'de
    /// olmak 429 seli + (Faz 2.3 oncesi) suresiz aski demek; dar varsayip genis
    /// tier'de olmak yalnizca YAVASLIK demek. Geri donusu olan tarafi seciyoruz.
    /// </summary>
    public const TrendyolRateLimitTier DefaultTier = TrendyolRateLimitTier.T50K;

    // Urun Okuma (tum sorgu uclari BIRLIKTE) - dakikalik
    private static int ProductReadLimit(TrendyolRateLimitTier tier) => tier switch
    {
        TrendyolRateLimitTier.T75K => 1250,
        TrendyolRateLimitTier.T150K => 1500,
        TrendyolRateLimitTier.T500K => 1750,
        TrendyolRateLimitTier.Unlimited => 2000,
        _ => 1000   // T50K
    };

    // Urun Yazma (create/update/delete BIRLIKTE) - dakikalik
    private static int ProductWriteLimit(TrendyolRateLimitTier tier) => tier switch
    {
        TrendyolRateLimitTier.T75K => 300,
        TrendyolRateLimitTier.T150K => 400,
        TrendyolRateLimitTier.T500K => 500,
        TrendyolRateLimitTier.Unlimited => 600,
        _ => 200    // T50K
    };

    // Stok & Fiyat Yazma - dakikalik
    private static int StockPriceWriteLimit(TrendyolRateLimitTier tier) => tier switch
    {
        TrendyolRateLimitTier.T75K => 500,
        TrendyolRateLimitTier.T150K => 1000,
        TrendyolRateLimitTier.T500K => 1500,
        TrendyolRateLimitTier.Unlimited => 2000,
        _ => 350    // T50K
    };

    // ########################################################################
    // SIPARIS KOVALARI DA TIER'E BAGLI (28.08.2026 duzeltmesi)
    //
    // URUN gecis planinin Faz 2'si "siparis kovalari tier'den etkilenmez" diyordu
    // ve bu konfig o varsayimla yazilmisti (her tier'de sabit 30/dk). RESMI TABLO
    // bunu YALANLIYOR: developers.trendyol.com/docs/1-servis-limitleri
    // "Get Shipment Packages" satiri 30 / 40 / 50 / 100 / 100 diyor.
    //
    // Pratik etkisi: HyperCep (193500) 53.818 listelemeyle T50K'da OLAMAZ; kod onu
    // 30/dk sayarken hakki 40/dk. Faz 3 tarih pencerelerini daraltip istek sayisini
    // artiracagi icin bu fark onemli hale geliyor.
    // ########################################################################

    /// <summary>
    /// "Limitsiz" tier'ler icin pratik tavan.
    ///
    /// NEDEN int.MaxValue DEGIL: Lua betigi INCR + karsilastirma yapiyor.
    /// 100.000/dk pratikte limitsizdir ve bir hata dongusu durumunda Trendyol'u
    /// kontrolsuzce dovmek yerine bir tavan birakir (IP/hesap yaptirimi riski).
    /// </summary>
    private const int PracticallyUnlimited = 100_000;

    /// <summary>Resmi tablo: "Get Shipment Packages" -> 30 / 40 / 50 / 100 / 100.</summary>
    private static int ShipmentPackagesLimit(TrendyolRateLimitTier tier) => tier switch
    {
        TrendyolRateLimitTier.T75K => 40,
        TrendyolRateLimitTier.T150K => 50,
        TrendyolRateLimitTier.T500K => 100,
        TrendyolRateLimitTier.Unlimited => 100,
        _ => 30     // T50K
    };

    /// <summary>
    /// Resmi tablo satirlari: "Kargo Takip Kodu Bildirme" ve "Paket Statu Bildirimi".
    /// Ikisi de 300 / 300 / 500 / limitsiz / limitsiz -> tek fonksiyondan beslenirler.
    /// </summary>
    private static int PackageNotifyLimit(TrendyolRateLimitTier tier) => tier switch
    {
        TrendyolRateLimitTier.T150K => 500,
        TrendyolRateLimitTier.T500K => PracticallyUnlimited,
        TrendyolRateLimitTier.Unlimited => PracticallyUnlimited,
        _ => 300    // T50K, T75K
    };

    /// <summary>
    /// Resmi tablo satirlari: "Siparis Paketlerini Bolme" (/split, /multi-split,
    /// /quantity-split, /split-packages - dordu de ayni) VE "Desi ve Koli Bilgisi
    /// Bildirimi". Hepsi 100 / 100 / 200 / limitsiz / limitsiz.
    ///
    /// SplitPackages ve BoxInfo ayni fonksiyondan besleniyor cunku degerleri
    /// TESADUFEN degil, resmi tabloda GERCEKTEN esit.
    /// </summary>
    private static int SplitAndBoxInfoLimit(TrendyolRateLimitTier tier) => tier switch
    {
        TrendyolRateLimitTier.T150K => 200,
        TrendyolRateLimitTier.T500K => PracticallyUnlimited,
        TrendyolRateLimitTier.Unlimited => PracticallyUnlimited,
        _ => 100    // T50K, T75K
    };

    /// <summary>
    /// Tier = saticinin LISTELEME KOTASI; kota adlari dogrudan tier adlaridir
    /// (Limit 50000, Limit 75000, ...). Urun sayisi bir tier'e SIGMIYORSA satici
    /// o tier'de OLAMAZ -> buradan bir ALT SINIR cikarilir.
    ///
    /// YALNIZ ALT SINIR: satici daha genis bir kota satin almis olabilir, bunu
    /// bilemeyiz. Hatanin maliyeti ASIMETRIK oldugu icin bilerek dar tarafta kaliyoruz:
    ///   dar tahmin   -> yalnizca YAVASLIK (geri donusu var)
    ///   genis tahmin -> 429 seli (geri donusu yok)
    ///
    /// 28.08.2026 canli olcumu: 193500 -> 53.750 + 68 = 53.818 -> T75K (30 yerine 40/dk).
    /// Diger uc magaza 50.000'in altinda -> T50K.
    ///
    /// DIKKAT: bu bir CIKARIM, olcum degildir. Canlida 429 gorulurse tahmin yuksek
    /// demektir; o magaza icin en dar tier'e dusulur (bkz. RedisRateLimiter downgrade).
    /// </summary>
    public static TrendyolRateLimitTier InferTier(int listingCount) => listingCount switch
    {
        > 500_000 => TrendyolRateLimitTier.Unlimited,
        > 150_000 => TrendyolRateLimitTier.T500K,
        > 75_000 => TrendyolRateLimitTier.T150K,
        > 50_000 => TrendyolRateLimitTier.T75K,
        _ => TrendyolRateLimitTier.T50K,     // varsayilan: EN DAR
    };

    /// <summary>
    /// Verilen tier icin tam kural tablosunu uretir.
    ///
    /// URUN ve SIPARIS kovalari tier'e BAGLIDIR.
    /// IADE / FINANS / QnA kovalari tier'den ETKILENMEZ - resmi tabloda onlar tier'siz;
    /// degerleri her tier'de aynidir.
    ///
    /// DUZELTME (28.08.2026): bu ozet eskiden "siparis ... tier'den ETKILENMEZ" diyordu.
    /// Yanlisti; resmi tablo "Get Shipment Packages" icin 30/40/50/100/100 veriyor.
    /// </summary>
    public static IReadOnlyDictionary<string, RateLimitRule> GetRules(TrendyolRateLimitTier tier)
    {
        var minute = TimeSpan.FromMinutes(1);

        return new Dictionary<string, RateLimitRule>
        {
            // --- Urun Servisleri (tier'e bagli, SATICI BAZLI ortak kovalar) ---
            [TrendyolRateLimitCategories.ProductRead]         = new(ProductReadLimit(tier), minute),
            [TrendyolRateLimitCategories.ProductWrite]        = new(ProductWriteLimit(tier), minute),
            [TrendyolRateLimitCategories.StockPriceWrite]     = new(StockPriceWriteLimit(tier), minute),

            // --- Ortak KATALOG uclari (tier'den BAGIMSIZ, GLOBAL kova) ---
            // Kategori agaci / kategori-ozellik / ozellik degerleri / markalar.
            // Saticiya ozgu degil, kimlik bilgisi bile istemiyorlar -> hicbir saticinin
            // kotasindan harcamazlar (bkz. TrendyolRateLimitCategories.CatalogRead).
            //
            // ############ 600 IDI, 1200'E CIKARILDI (28.08.2026) ############
            // Eski gerekce: "canli olcumde ~400 istek/dk'da 429 gorulmedi". Dogru ama
            // O OLCUM SIRADAN (tek akisli) kodun ulasabildigi hizdi - yani limit degil,
            // KENDI YAVASLIGIMIZ olculmustu.
            //
            // Katalog tazelemesi (kategori, ozellik) CIFTI bazina gecince ~25.300 istek
            // gerekiyor; 600/dk = 10/sn bunu 42 dakikaya cikarir ve eszamanliligi bogar.
            //
            // Gercek tolerans olculdu (gercek cift ornekleriyle, 120 istekli turlar):
            //     eszaman= 1 ->   7,2 istek/sn   429=0  5xx=0
            //     eszaman= 8 ->  34,9 istek/sn   429=0  5xx=0
            //     eszaman=16 ->  47,0 istek/sn   429=0  5xx=0
            //     eszaman=32 -> 131,6 istek/sn   429=0  5xx=0
            //
            // 1200/dk = 20/sn secildi: olculen en dusuk temiz seviyenin (35/sn) ALTINDA.
            // Olcum benim makinemden ve PATLAMA seklinde yapildi; sunucudan SUREKLI
            // 25.000 istekte ucun davranisi ayni olmayabilir, o yuzden yarisindan az.
            //
            // Tavan iki isi birden yapiyor: (1) bir hata dongusunun Trendyol'u doverek
            // IP/hesap seviyesinde yaptirim cekmesini onler, (2) tur suresini
            // ongorulebilir kilar. SATICI KOTASINA DOKUNMAZ.
            //
            // 429 gorulurse ILK azaltilacak deger burasidir (ve CategoriesWorker
            // .MaxConcurrentFetches).
            // ###############################################################
            [TrendyolRateLimitCategories.CatalogRead]         = new(1200, minute),

            // Urun grubunda DEGIL; kendi sert limiti var.
            [TrendyolRateLimitCategories.SupplierAddresses]   = new(1, TimeSpan.FromHours(1)),

            // --- Siparis Servisleri (TIER'E BAGLI - 28.08.2026 duzeltmesi) ---
            // Bu bes kova ONCEDEN her tier'de sabitti; resmi tablo tier'e bagli oldugunu
            // soyluyor. Bkz. ShipmentPackagesLimit / PackageNotifyLimit / SplitAndBoxInfoLimit.
            [TrendyolRateLimitCategories.ShipmentPackages]    = new(ShipmentPackagesLimit(tier), minute),
            [TrendyolRateLimitCategories.TrackingNumber]      = new(PackageNotifyLimit(tier), minute),

            // NOT: PackageStatus kovasinda fatura linki uclari da var (SendInvoiceLinkAsync,
            // DeleteInvoiceLinkAsync). Resmi tabloda FATURA LINKI SATIRI YOK - onlarin bu
            // kovaya konmasi belgelenmemis bir varsayimdir. Cagiranlari olmadigi icin bugun
            // etkisiz; bilincli olarak DEGISTIRILMEDI (bkz. Faz 6.5).
            [TrendyolRateLimitCategories.PackageStatus]       = new(PackageNotifyLimit(tier), minute),

            [TrendyolRateLimitCategories.SplitPackages]       = new(SplitAndBoxInfoLimit(tier), minute),
            [TrendyolRateLimitCategories.BoxInfo]             = new(SplitAndBoxInfoLimit(tier), minute),

            // --- Iade Servisleri (tier'den bagimsiz) ---
            [TrendyolRateLimitCategories.ClaimsList]          = new(1000, minute),
            [TrendyolRateLimitCategories.ClaimApprove]        = new(5, minute),

            // --- Muhasebe ve Finans Servisleri (tier'den bagimsiz) ---
            [TrendyolRateLimitCategories.InvoiceSettlements]  = new(100, minute),
            [TrendyolRateLimitCategories.InvoiceCargo]        = new(100, minute),

            // --- QnA Servisleri (tier'den bagimsiz) ---
            [TrendyolRateLimitCategories.QnAFilter]           = new(1000, minute),
            [TrendyolRateLimitCategories.QnAAnswer]           = new(500, minute),
        };
    }

    /// <summary>
    /// Konfigten gelen tier metninin durumu.
    ///
    /// "Belirtilmemis" ile "gecersiz" AYRI tutulur cunku ikisi farkli seyler:
    /// - Belirtilmemis: bilincli varsayilan. Canli sunucularda appsettings ELLE
    ///   yonetiliyor ve deploy repo'daki dosyayi artefakttan SILIYOR, yani bolumun
    ///   olmamasi NORMAL bir durum. Her aciliste uyari basmak gurultu olurdu.
    /// - Gecersiz: birisi tier yazmaya CALISTI ve tutturamadi ("T60K", "50k"...).
    ///   Bu sessiz kalirsa yanlis limitle gunlerce calisilir ve kimse nedenini bilmez.
    /// </summary>
    public enum TierConfigState
    {
        /// <summary>Konfigte deger var ve tanindi.</summary>
        Valid,

        /// <summary>Konfigte deger yok/bos. Varsayilan uygulandi (beklenen durum).</summary>
        NotConfigured,

        /// <summary>Konfigte deger var ama taninmadi. Varsayilan uygulandi (DIKKAT).</summary>
        Invalid
    }

    /// <summary>
    /// Konfigten gelen metni tier'e cevirir. Taninmayan/bos deger
    /// <see cref="DefaultTier"/>'a duser; <paramref name="state"/> cagiran tarafin
    /// dogru log seviyesini secebilmesi icin nedeni bildirir.
    /// </summary>
    public static TrendyolRateLimitTier ParseTier(string? raw, out TierConfigState state)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            state = TierConfigState.NotConfigured;
            return DefaultTier;
        }

        if (Enum.TryParse<TrendyolRateLimitTier>(raw.Trim(), ignoreCase: true, out var tier)
            && Enum.IsDefined(typeof(TrendyolRateLimitTier), tier))
        {
            state = TierConfigState.Valid;
            return tier;
        }

        state = TierConfigState.Invalid;
        return DefaultTier;
    }
}
