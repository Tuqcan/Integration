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

    /// <summary>
    /// Verilen tier icin tam kural tablosunu uretir.
    ///
    /// SIPARIS / IADE / FINANS / QnA kovalari tier'den ETKILENMEZ - yeni rejim
    /// yalnizca urun servislerini kapsiyor. Onlarin degerleri her tier'de aynidir.
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
            // 600/dk NEDEN: canli olcumde ~400 istek/dk'da 429 GORULMEDI, ama "429
            // gormedik" != "limit yok". Tavan iki isi birden yapiyor: (1) bir hata
            // dongusunun Trendyol'u doverek IP/hesap seviyesinde yaptirim cekmesini
            // onler, (2) tur suresini ongorulebilir kilar. ~3.400 katalog istegi bu
            // tavanda ~6 dakikada biter ve SATICI KOTASINA DOKUNMAZ.
            [TrendyolRateLimitCategories.CatalogRead]         = new(600, minute),

            // Urun grubunda DEGIL; kendi sert limiti var.
            [TrendyolRateLimitCategories.SupplierAddresses]   = new(1, TimeSpan.FromHours(1)),

            // --- Siparis Servisleri (tier'den bagimsiz) ---
            // ShipmentPackages: 8 Haziran 2026'dan itibaren yeni limit (50000 tier) -> 30/min (eski 2000/min)
            [TrendyolRateLimitCategories.ShipmentPackages]    = new(30, minute),
            [TrendyolRateLimitCategories.TrackingNumber]      = new(300, minute),
            [TrendyolRateLimitCategories.PackageStatus]       = new(300, minute),
            [TrendyolRateLimitCategories.SplitPackages]       = new(100, minute),
            [TrendyolRateLimitCategories.BoxInfo]             = new(100, minute),

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
