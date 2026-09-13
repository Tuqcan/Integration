namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;

public enum EnumTransactionType
{
    // ############ ISTEK / YANIT AYRIMI ############
    // Bu enum IKI ISI birden goruyor ve ikisinin deger kumesi AYNI DEGIL:
    //   1) ISTEK  : `?transactionType=<uye adi>` -> Trendyol yalnizca kendi
    //               listesindeki 25 degeri kabul eder. Tanimsiz her deger
    //               HTTP 400 "Lütfen Belirlenmiş Tiplerden" dondurur
    //               (RefurbishedSale / Bogus123 / bos -> hepsi 400, 13.09.2026 canli olcum).
    //   2) YANIT  : `transactionType` alani TURKCE etiket doner ("Satış", "İade" ...)
    //               ve Trendyol buraya ISTEKTE KABUL ETMEDIGI degerler de yazabiliyor.
    //
    // Bu yuzden yalnizca-yanit uyeleri EnumTransactionTypeRules.ResponseOnly
    // kumesinde isaretlenir; istek listesi kuran kod O KUMEYI DISLAMAK ZORUNDA.
    // Yeni uye eklerken once sor: Trendyol bunu ISTEK olarak kabul ediyor mu?
    // #############################################
    Unknown = 0,                      // Bilinmeyen islem tipi - converter fallback
    Sale = 1,                         // Ürün satışı (Alacak)
    Return = 2,                       // Ürün iadesi (Borç)
    Discount = 3,                     // Tedarikçi indirimi (Borç)
    DiscountCancel = 4,              // İndirim iptali (Alacak)
    Coupon = 5,                       // Tedarikçi kuponu (Borç)
    CouponCancel = 6,                // Kupon iptali (Alacak)
    ProvisionPositive = 7,           // Pozitif provizyon (Alacak)
    ProvisionNegative = 8,           // Negatif provizyon (Borç)
    TYDiscount = 9,                  // Trendyol promosyon indirimi (Borç)
    TYDiscountCancel = 10,           // Trendyol promosyon indirimi iptali (Alacak)
    TYCoupon = 11,                   // Trendyol kuponu (Borç)
    TYCouponCancel = 12,             // Trendyol kuponu iptali (Alacak)
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
    DeliveryFeeCancel = 24,          // Kargo ücreti iptali (Alacak)
    PayByLink = 25,                  // Link ile ödeme

    // YALNIZCA YANIT. Trendyol bu kayitlari `transactionType=PayByLink` kovasindan
    // dondurur ama yanitta "Yenilenmiş Satış" yazar. Dokumanda/changelog'da GECMEZ.
    // Istekte kullanilamaz -> ResponseOnly kumesinde (bkz. EnumTransactionTypeRules).
    RefurbishedSale = 26             // Yenilenmiş Satış (iade sonrasi yenilenip yeniden satilan urun)
}

/// <summary>
/// Enum uyelerinin Trendyol API'sinde nasil kullanilabilecegini tanimlar.
///
/// NEDEN AYRI BIR KUME: istek listesi <c>Enum.GetValues(...)</c> ile kuruluyor.
/// Yalnizca-yanit bir uye eklenip burada isaretlenmezse, istek listesine sizar ve
/// HER tedarikci x HER 15 gunluk pencere x HER turda HTTP 400 uretir (3 retry +
/// hata maili + bosa giden kota). 400 "fatal" sayilmadigi icin API anahtari
/// deaktive olmaz — yani hata SESSIZ kalir, yalnizca gurultu ve israf olarak birikir.
/// </summary>
public static class EnumTransactionTypeRules
{
    /// <summary>Trendyol'un ISTEK parametresi olarak KABUL ETMEDIGI uyeler.</summary>
    private static readonly HashSet<EnumTransactionType> ResponseOnly =
    [
        EnumTransactionType.Unknown,          // converter fallback, gercek bir tip degil
        EnumTransactionType.RefurbishedSale   // yanitta gelir, istekte 400
    ];

    /// <summary>Bu uye `?transactionType=` parametresinde kullanilabilir mi?</summary>
    public static bool IsRequestable(EnumTransactionType type) => !ResponseOnly.Contains(type);

    /// <summary>Trendyol'a sorulabilecek tum tipler (istek listesi kuranlar BUNU kullanir).</summary>
    public static IReadOnlyList<EnumTransactionType> RequestableTypes { get; } =
        Enum.GetValues<EnumTransactionType>().Where(IsRequestable).ToList();
}
