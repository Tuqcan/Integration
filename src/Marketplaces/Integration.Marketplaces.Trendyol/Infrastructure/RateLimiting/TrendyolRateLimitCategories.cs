namespace Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

public static class TrendyolRateLimitCategories
{
    // Urun Servisleri
    public const string ProductCreate = "ProductCreate";               // 1000/min
    public const string ProductUpdate = "ProductUpdate";               // 1000/min
    // StockPriceUpdate -> NO LIMIT (tanimsiz = limitsiz)
    public const string BatchCheck = "BatchCheck";                     // 1000/min
    public const string ProductFilter = "ProductFilter";               // 2000/min
    public const string ProductDelete = "ProductDelete";               // 100/min
    public const string SupplierAddresses = "SupplierAddresses";       // 1/hour
    public const string Brands = "Brands";                             // 50/min
    public const string Categories = "Categories";                     // 50/min
    public const string CategoryAttributes = "CategoryAttributes";     // 50/min

    // Siparis Servisleri (Limit 50000 tier)
    public const string ShipmentPackages = "ShipmentPackages";         // 2000/min
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
}
