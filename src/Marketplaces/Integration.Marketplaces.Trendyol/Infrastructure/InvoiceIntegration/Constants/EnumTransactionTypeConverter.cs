using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;

public class EnumTransactionTypeConverter : JsonConverter<EnumTransactionType>
{
    private static readonly Dictionary<string, EnumTransactionType> _map = new(StringComparer.OrdinalIgnoreCase)
{
    { "Satış", EnumTransactionType.Sale },
    { "İade", EnumTransactionType.Return },
    { "İndirim", EnumTransactionType.Discount },
    { "İndirim İptal", EnumTransactionType.DiscountCancel },
    { "Kupon", EnumTransactionType.Coupon },
    { "Kupon İptal", EnumTransactionType.CouponCancel },
    { "Provizyon +", EnumTransactionType.ProvisionPositive },
    { "Provizyon -", EnumTransactionType.ProvisionNegative },
    { "Kurumsal Fatura - Trendyol Promosyon", EnumTransactionType.TyDiscount },
    { "Kurumsal Fatura - Trendyol Promosyon İptali", EnumTransactionType.TyDiscountCancel },
    { "Kurumsal Fatura - Trendyol Kupon", EnumTransactionType.TyCoupon },
    { "Kurumsal Fatura - Trendyol Kupon İptali", EnumTransactionType.TyCouponCancel },
    { "Kısmi İade", EnumTransactionType.ManualRefund },
    { "Kısmi İade İptal", EnumTransactionType.ManualRefundCancel },
    { "Kargo Ücreti", EnumTransactionType.DeliveryFee },
    { "Kargo Ücreti İptal", EnumTransactionType.DeliveryFeeCancel },
    { "Hakediş Pozitif Düzeltme", EnumTransactionType.SellerRevenuePositive },
    { "Hakediş Negatif Düzeltme", EnumTransactionType.SellerRevenueNegative },
    { "Komisyon Pozitif Düzeltme", EnumTransactionType.CommissionPositive },
    { "Komisyon Negatif Düzeltme", EnumTransactionType.CommissionNegative },
    { "Hakediş Pozitif İptal", EnumTransactionType.SellerRevenuePositiveCancel },
    { "Hakediş Negatif İptal", EnumTransactionType.SellerRevenueNegativeCancel },
    { "Komisyon Pozitif İptal", EnumTransactionType.CommissionPositiveCancel },
    { "Komisyon Negatif İptal", EnumTransactionType.CommissionNegativeCancel }
        

};

    public override EnumTransactionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (_map.TryGetValue(value ?? "", out var result))
            return result;

        throw new JsonException($"Geçersiz transactionType: {value}");
    }

    public override void Write(Utf8JsonWriter writer, EnumTransactionType value, JsonSerializerOptions options)
    {
        var stringValue = _map.FirstOrDefault(x => x.Value == value).Key ?? value.ToString();
        writer.WriteStringValue(stringValue);
    }
}
