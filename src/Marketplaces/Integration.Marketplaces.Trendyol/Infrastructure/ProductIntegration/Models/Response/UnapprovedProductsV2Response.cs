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

    public decimal? DimensionalWeight { get; set; }
    public string? StockCode { get; set; }

    public List<FilterProductImageResponseModel> Images { get; set; } = [];
    public List<FilterProductAttributeResponseModel> Attributes { get; set; } = [];
}
