
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
    { "Stopaj", EnumOtherFinancialTransactionType.Stoppage }
};

    public override EnumOtherFinancialTransactionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (_map.TryGetValue(value ?? "", out var result))
            return result;

        throw new JsonException($"Geçersiz otherFinancial transactionType: {value}");
    }

    public override void Write(Utf8JsonWriter writer, EnumOtherFinancialTransactionType value, JsonSerializerOptions options)
    {
        var stringValue = _map.FirstOrDefault(x => x.Value == value).Key ?? value.ToString();
        writer.WriteStringValue(stringValue);
    }
}
