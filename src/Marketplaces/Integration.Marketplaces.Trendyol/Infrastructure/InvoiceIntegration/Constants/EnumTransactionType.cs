namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;

public enum EnumTransactionType
{
    Sale = 1,                         // Ürün satışı (Alacak)
    Return = 2,                       // Ürün iadesi (Borç)
    Discount = 3,                     // Tedarikçi indirimi (Borç)
    DiscountCancel = 4,              // İndirim iptali (Alacak)
    Coupon = 5,                       // Tedarikçi kuponu (Borç)
    CouponCancel = 6,                // Kupon iptali (Alacak)
    ProvisionPositive = 7,           // Pozitif provizyon (Alacak)
    ProvisionNegative = 8,           // Negatif provizyon (Borç)
    TyDiscount = 9,                  // Trendyol promosyon indirimi (Borç)
    TyDiscountCancel = 10,           // Trendyol promosyon indirimi iptali (Alacak)
    TyCoupon = 11,                   // Trendyol kuponu (Borç)
    TyCouponCancel = 12,             // Trendyol kuponu iptali (Alacak)
    SellerRevenuePositive = 13,      // Satıcı hakediş artışı (Alacak)
    SellerRevenueNegative = 14,      // Satıcı hakediş kesintisi (Borç)
    CommissionPositive = 15,         // Ekstra komisyon kesintisi (Borç)
    CommissionNegative = 16,         // Komisyon indirimi / iadesi (Alacak)
    SellerRevenuePositiveCancel = 17,// Hakediş artışı iptali (Borç)
    SellerRevenueNegativeCancel = 18,// Hakediş kesintisi iptali (Alacak)
    CommissionPositiveCancel = 19,   // Ekstra komisyon iptali (Alacak)
    CommissionNegativeCancel = 20,   // Komisyon indirimi iptali (Borç)
    ManualRefund = 21,               // Kısmi iade (Borç)
    ManualRefundCancel = 22,         // Kısmi iade iptali (Alacak)
    DeliveryFee = 23,                // Kargo ücreti kesintisi (Borç)
    DeliveryFeeCancel = 24           // Kargo ücreti iptali (Alacak)
}
