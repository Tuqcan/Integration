namespace Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

public record RateLimitRule(int Limit, TimeSpan Window);

public static class TrendyolRateLimitConfig
{
    // Trendyol'un yayinladigi resmi limitler (birebir)
    public static readonly Dictionary<string, RateLimitRule> Rules = new()
    {
        // Urun Servisleri
        [TrendyolRateLimitCategories.ProductCreate]       = new(1000, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.ProductUpdate]       = new(1000, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.BatchCheck]          = new(1000, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.ProductFilter]       = new(2000, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.ProductDelete]       = new(100, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.SupplierAddresses]   = new(1, TimeSpan.FromHours(1)),
        [TrendyolRateLimitCategories.Brands]              = new(50, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.Categories]          = new(50, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.CategoryAttributes]  = new(50, TimeSpan.FromMinutes(1)),

        // Siparis Servisleri (Limit 50000 tier)
        // ShipmentPackages: 8 Haziran 2026'dan itibaren yeni limit (50000 tier) -> 30/min (eski 2000/min)
        [TrendyolRateLimitCategories.ShipmentPackages]    = new(30, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.TrackingNumber]      = new(300, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.PackageStatus]       = new(300, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.SplitPackages]       = new(100, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.BoxInfo]             = new(100, TimeSpan.FromMinutes(1)),

        // Iade Servisleri
        [TrendyolRateLimitCategories.ClaimsList]          = new(1000, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.ClaimApprove]        = new(5, TimeSpan.FromMinutes(1)),

        // Muhasebe ve Finans Servisleri
        [TrendyolRateLimitCategories.InvoiceSettlements]  = new(100, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.InvoiceCargo]        = new(100, TimeSpan.FromMinutes(1)),

        // QnA (Müşteri Soruları) Servisleri
        [TrendyolRateLimitCategories.QnAFilter]           = new(1000, TimeSpan.FromMinutes(1)),
        [TrendyolRateLimitCategories.QnAAnswer]           = new(500, TimeSpan.FromMinutes(1)),
    };
}
