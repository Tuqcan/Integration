
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;

public class EnumOtherFinancialTransactionTypeConverter : JsonConverter<EnumOtherFinancialTransactionType>
{
    private static readonly Dictionary<string, EnumOtherFinancialTransactionType> _map = new(StringComparer.OrdinalIgnoreCase)
{
    { "Ön Ödeme", EnumOtherFinancialTransactionType.CashAdvance },
    { "Virman Ödemesi", EnumOtherFinancialTransactionType.WireTransfer },
    { "Gelen Havale", EnumOtherFinancialTransactionType.IncomingTransfer },
    { "İade Faturası", EnumOtherFinancialTransactionType.ReturnInvoice },
    { "Komisyon Mutabakat Faturası", EnumOtherFinancialTransactionType.CommissionAgreementInvoice },
    { "Ödeme", EnumOtherFinancialTransactionType.PaymentOrder },
    { "Fatura", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Finansal Kalem", EnumOtherFinancialTransactionType.FinancialItem },
    { "Stopaj", EnumOtherFinancialTransactionType.Stoppage },
    { "E-ticaret Stopajı", EnumOtherFinancialTransactionType.Stoppage },
    { "Tahsilat Fişi", EnumOtherFinancialTransactionType.PaymentOrder },
    { "Kusurlu Ürün Faturası", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Komisyon Faturası", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Kargo Fatura", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Platform Hizmet Bedeli", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Tedarik Edememe", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Termin Gecikme Bedeli", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Eksik Ürün Faturası", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Yanlış Ürün Faturası", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "Reklam Bedeli", EnumOtherFinancialTransactionType.DeductionInvoices },
    { "AZ-Uluslararası Hizmet Bedeli", EnumOtherFinancialTransactionType.FinancialItem },
    { "AZ-Platform Hizmet Bedeli", EnumOtherFinancialTransactionType.FinancialItem },
    { "AZ-Gecikme Cezası", EnumOtherFinancialTransactionType.FinancialItem },
    { "AZ-Yurtdışı Operasyon Bedeli", EnumOtherFinancialTransactionType.FinancialItem },
    { "Yurtdışı Operasyon Iade Bedeli", EnumOtherFinancialTransactionType.FinancialItem },
    { "Uluslararası Hizmet Bedeli", EnumOtherFinancialTransactionType.FinancialItem }
};

    public override EnumOtherFinancialTransactionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (_map.TryGetValue(value ?? "", out var result))
            return result;

        return EnumOtherFinancialTransactionType.Unknown;
    }

    public override void Write(Utf8JsonWriter writer, EnumOtherFinancialTransactionType value, JsonSerializerOptions options)
    {
        var stringValue = _map.FirstOrDefault(x => x.Value == value).Key ?? value.ToString();
        writer.WriteStringValue(stringValue);
    }
}
