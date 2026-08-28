namespace Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

/// <summary>
/// Saticinin Trendyol LISTELEME KOTASI tier'i. 14.09.2026'dan itibaren urun
/// servislerinin dakikalik limitleri bu tier'e gore degisiyor.
///
/// Tier satici panelinden ogrenilir ve appsettings'e yazilir
/// ("Trendyol": { "RateLimitTier": "T50K" }).
/// </summary>
public enum TrendyolRateLimitTier
{
    /// <summary>50.000 urun listeleme kotasi. VARSAYILAN (en dar).</summary>
    T50K = 0,
    T75K = 1,
    T150K = 2,
    T500K = 3,

    /// <summary>Limitsiz listeleme kotasi. "Limitsiz" kota demek, limitsiz ISTEK demek DEGIL.</summary>
    Unlimited = 4
}
