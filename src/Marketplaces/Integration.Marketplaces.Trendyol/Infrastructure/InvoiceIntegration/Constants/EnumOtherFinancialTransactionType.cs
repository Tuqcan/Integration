namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;

public enum EnumOtherFinancialTransactionType
{
    CashAdvance = 1,                  // Vadesi gelmemiş hakediş için erken ödeme
    WireTransfer = 2,                 // Trendyol ile satıcı arasında virman işlemi
    IncomingTransfer = 3,            // Tedarikçiden Trendyol’a yapılan ödeme
    ReturnInvoice = 4,               // Tedarikçiden Trendyol’a kesilen iade faturası
    CommissionAgreementInvoice = 5,  // Komisyon mutabakatı faturası (iade sonrası)
    PaymentOrder = 6,                // Hakediş ödemesi
    DeductionInvoices = 7,           // Hizmet bedeli gibi kesilen Trendyol faturaları
    FinancialItem = 8,               // Diğer finansal işlemler (ekstra madde)
    Stoppage = 9                     // E-ticaret stopajı ve iptalleri
}
