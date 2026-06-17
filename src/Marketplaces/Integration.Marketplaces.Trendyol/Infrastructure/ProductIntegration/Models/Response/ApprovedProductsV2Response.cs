using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;

// GET .../products/approved (V2) response. Nested: content -> variants[].
// Sadece flatten'da kullanilan alanlar modellenmistir; System.Text.Json eslesmeyen
// JSON alanlarini yok sayar.
public class ApprovedProductsV2Response : PaginationModel, IResponseModel
{
    public string? NextPageToken { get; set; }
    public List<ApprovedProductV2> Content { get; set; } = [];
}

public class ApprovedProductV2 : IResponseModel
{
    public long ContentId { get; set; }
    public string ProductMainId { get; set; }

    public ProductBrandV2 Brand { get; set; }
    public ProductCategoryV2 Category { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public List<FilterProductImageResponseModel> Images { get; set; } = [];
    public List<FilterProductAttributeResponseModel> Attributes { get; set; } = [];
    public List<ApprovedVariantV2> Variants { get; set; } = [];
}

public class ProductBrandV2 : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ProductCategoryV2 : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ApprovedVariantV2 : IResponseModel
{
    public long VariantId { get; set; }
    public int SupplierId { get; set; }
    public string Barcode { get; set; }
    public decimal? Commission { get; set; }

    public string? ProductUrl { get; set; }
    public string? StockCode { get; set; }

    public bool OnSale { get; set; }
    public bool Locked { get; set; }
    public bool Archived { get; set; }
    public bool Blacklisted { get; set; }

    public int? VatRate { get; set; }   // Trendyol null donebiliyor -> nullable

    // V1 createDateTime/lastUpdateDate karsiligi (birebir ayni epoch ms).
    public long SellerCreatedDate { get; set; }
    public long SellerModifiedDate { get; set; }

    public ProductDeliveryOptionsV2? DeliveryOptions { get; set; }
    public ProductStockV2? Stock { get; set; }
    public ProductPriceV2? Price { get; set; }
}

public class ProductDeliveryOptionsV2 : IResponseModel
{
    public int? DeliveryDuration { get; set; }
}

public class ProductStockV2 : IResponseModel
{
    public int? Quantity { get; set; }
}

public class ProductPriceV2 : IResponseModel
{
    public decimal? SalePrice { get; set; }
    public decimal? ListPrice { get; set; }
}
