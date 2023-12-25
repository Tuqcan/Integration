using Integration.Hub;

namespace Integration.PaymentGateways.Sipay.Infrastructure.PaymentIntegration.Models.Request;

public class NonSecurePaymentRequestModel : IRequestModel
{
    public NonSecurePaymentRequestModel(string ccHolderName, string ccNo, int expiryMonth, int expiryYear, int cvv, string currencyCode, int installmentsNumber, int invoiceId, string invoiceDescription, string name, string surname, int total, string items, string cancelUrl, string returnUrl, string hashKey, string ip, int orderType)
    {
        CCHolderName = ccHolderName;
        CCNo = ccNo;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        CVV = cvv;
        CurrencyCode = currencyCode;
        InstallmentsNumber = installmentsNumber;
        InvoiceId = invoiceId;
        InvoiceDescription = invoiceDescription;
        Name = name;
        Surname = surname;
        Total = total;
        Items = items;
        CancelUrl = cancelUrl;
        ReturnUrl = returnUrl;
        HashKey = hashKey;
        Ip = ip;
        OrderType = orderType;
    }

    public string CCHolderName { get; set; }
    public string CCNo { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public int CVV { get; set; }
    public string CurrencyCode { get; set; }
    public int InstallmentsNumber { get; set; }
    public int InvoiceId { get; set; }
    public string InvoiceDescription { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public int Total { get; set; }
    public string MerchantKey { get; set; }
    public string Items { get; set; }
    public string CancelUrl { get; set; }
    public string ReturnUrl { get; set; }
    public string HashKey { get; set; }
    public string Ip { get; set; }
    public int OrderType { get; set; }
    public int IsCommissionFromUser { get; set; }
    public string CommissionBy { get; set; }
    public string BillAddress1 { get; set; }
    public string BillAddress2 { get; set; }
    public string BillCity { get; set; }
    public string BillPostcode { get; set; }
    public string BillState { get; set; }
    public string BillCountry { get; set; }
    public string BillEmail { get; set; }
    public string BillPhone { get; set; }
    public string CardProgram { get; set; }
    public string TransactionType { get; set; }
    public string VposType { get; set; }
    public string IdentityNumber { get; set; }
    public string SaleWebHookKey { get; set; }
    public int RecurringPaymentNumber { get; set; }
    public string RecurringPaymentCycle { get; set; }
    public string RecurringPaymentInterval { get; set; }
    public string RecurringWebHookKey { get; set; }
}