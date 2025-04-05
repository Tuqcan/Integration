using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.JsonConverter;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Models.Response;

public class GetFinancialTransactionsResponseModel : PaginationModel
{
    public List<FinancialTransactionResponseModel>? Content { get; set; }
}

public class FinancialTransactionResponseModel : IResponseModel
{
    public string Id { get; set; } = null!;                         // İşlem ID
    public long TransactionDate { get; set; }                  // İşlem tarihi (Unix ms)

    [JsonConverter(typeof(EnumTransactionTypeConverter))]
    public EnumTransactionType TransactionType { get; set; }   // İşlem tipi (Enum)

    public string Barcode { get; set; }                        // Barkod
    public long? ReceiptId { get; set; }                       // Dekont numarası
    public string? Description { get; set; }                    // Açıklama

    public decimal Debt { get; set; }                           // Borç
    public decimal Credit { get; set; }                         // Alacak

    public int? PaymentPeriod { get; set; }                    // Vade süresi (gün)
    public decimal? CommissionRate { get; set; }                // Komisyon oranı
    public decimal? CommissionAmount { get; set; }              // Komisyon tutarı
    public string CommissionInvoiceSerialNumber { get; set; }  // Komisyon fatura no

    public decimal? SellerRevenue { get; set; }                 // Satıcı net kazancı
    public string OrderNumber { get; set; }                    // Sipariş numarası
    [JsonConverter(typeof(IntToLongNullableConverter))]
    public long? PaymentOrderId { get; set; }                   // Ödeme emri ID
    public long? PaymentDate { get; set; }                     // Ödeme tarihi (Unix ms)

    public int SellerId { get; set; }                         // Tedarikçi ID
    public long? StoreId { get; set; }                         // Mağaza ID (null döner)
    public string StoreName { get; set; }                      // Mağaza adı
    public string StoreAddress { get; set; }                   // Mağaza adresi

    public string Country { get; set; }                        // Ülke (genellikle "Türkiye")
    public string Affiliate { get; set; }                      // TRENDYOLTR / TRENDYOLAZJV
    [JsonConverter(typeof(IntToLongNullableConverter))]
    public long? ShipmentPackageId { get; set; }                // Kargo paketi ID

    public long OrderDate { get; set; }                        // Sipariş tarihi (Unix ms)

    public Guid UserId { get; set; }                           // Kullanıcı ID
}
