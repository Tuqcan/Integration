using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;

// GET .../products/unapproved (V2) response. Düz yapı.
// NOT: Trendyol dökümanı "media[]" der ama gerçek response "images[]" kullanır (approved ile aynı).
public class UnapprovedProductsV2Response : PaginationModel, IResponseModel
{
    public string? NextPageToken { get; set; }
    public List<UnapprovedProductV2> Content { get; set; } = [];
}

public class UnapprovedProductV2 : IResponseModel
{
    public int SupplierId { get; set; }
    public string ProductMainId { get; set; }

    // "rejected" | "pendingApproval"
    public string? Status { get; set; }

    public long CreateDateTime { get; set; }
    public long LastUpdateDate { get; set; }

    public ProductBrandV2 Brand { get; set; }
    public ProductCategoryV2 Category { get; set; }

    public string Barcode { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public int? Quantity { get; set; }
    public decimal? ListPrice { get; set; }
    public decimal? SalePrice { get; set; }
    public int? VatRate { get; set; }   // Trendyol null donebiliyor -> nullable

    /// <summary>
    /// Urun mensei (ISO 3166-1 alpha-2, canli ornek: "CN").
    ///
    /// Trendyol menseiyi attribute'tan bagimsiz STANDART bir alana tasiyor ve
    /// 23.10.2026'dan itibaren urun YAZMA isteklerinde ZORUNLU olacak. Biz su an
    /// yazmadigimiz icin sert bir kirilma yok; okuma tarafinda alan zaten dolu geliyor
    /// (kanit: T1_approved_hypercep.json variant.origin, T3_unapproved_hypercep.json item.origin).
    ///
    /// CIFTE KAYNAK UYARISI: ayni bilgi content.attributes icinde attribute 1192
    /// ("Mensei", deger "CN") olarak DA geliyor. QnA promptu menseiyi O attribute'tan
    /// aliyor; bu alan promptta AYRICA gosterilMEZ (aksi halde "Mensei: CN" iki kez basar).
    /// Bu alanin amaci raporlama ve ileriye donuk yazma destegidir.
    /// </summary>
    public string? Origin { get; set; }

    public decimal? DimensionalWeight { get; set; }
    public string? StockCode { get; set; }

    public List<FilterProductImageResponseModel> Images { get; set; } = [];
    public List<FilterProductAttributeResponseModel> Attributes { get; set; } = [];
}
