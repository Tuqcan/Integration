using System.Text.Json.Serialization;
using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.JsonConverter;

namespace Integration.Marketplaces.Trendyol.Infrastructure.FinanceIntegration.Models.Response;

public class GetOtherFinancialTransactionsResponseModel : PaginationModel
{
    public List<OtherFinancialTransactionResponseModel>? Content { get; set; }
}

public class OtherFinancialTransactionResponseModel : IResponseModel
{
    public string Id { get; set; } = null!;                          // İşlem ID
    public long TransactionDate { get; set; }                  // İşlem tarihi (Unix ms)

    //[JsonConverter(typeof(EnumOtherFinancialTransactionTypeConverter))]
    public string TransactionType { get; set; } = null!;  // İşlem tipi (Enum)

    public string? Barcode { get; set; }                        // Barkod (genelde null)
    [JsonConverter(typeof(IntToLongNullableConverter))]
    public long? ReceiptId { get; set; }                       // Dekont no
    public string? Description { get; set; }                    // Açıklama

    public decimal Debt { get; set; }                           // Borç
    public decimal Credit { get; set; }                         // Alacak

    public int? PaymentPeriod { get; set; }                    // Vade süresi (genelde null)
    public decimal? CommissionRate { get; set; }                // Komisyon oranı
    public decimal? CommissionAmount { get; set; }              // Komisyon tutarı
    public string? CommissionInvoiceSerialNumber { get; set; }  // Komisyon fatura no

    public decimal? SellerRevenue { get; set; }                 // Satıcı net kazancı
    public string? OrderNumber { get; set; }                    // Sipariş numarası (genelde null)
    [JsonConverter(typeof(IntToLongNullableConverter))]
    public long? PaymentOrderId { get; set; }                   // Ödeme emri ID
    public long? PaymentDate { get; set; }                     // Ödeme tarihi (Unix ms)
 
    public int SellerId { get; set; }
    [JsonConverter(typeof(IntToLongNullableConverter))]// Tedarikçi ID
    public long? StoreId { get; set; }                         // Mağaza ID (null)
    public string? StoreName { get; set; }                      // Mağaza adı (null)
    public string? StoreAddress { get; set; }                   // Mağaza adresi (null)

    public string? Country { get; set; }                        // Ülke
    public string Affiliate { get; set; } = string.Empty;                    // TRENDYOLTR / TRENDYOLAZJV
    [JsonConverter(typeof(IntToLongNullableConverter))]
    public long? ShipmentPackageId { get; set; }                // Paket ID (bazı işlemlerde null olabilir)

    public long? OrderDate { get; set; }                        // Sipariş tarihi (bazı işlemlerde null olabilir)

    public Guid UserId { get; set; }                           // Kullanıcı ID
}
