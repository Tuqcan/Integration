namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;

// V2 /products/approved endpoint'inin desteklediği tarih sorgu tipleri.
// V1/unapproved'daki EnumdateQueryType (CREATED_DATE/LAST_MODIFIED_DATE) ile karışmaması için ayrıdır.
// Incremental senkronda VARIANT_MODIFIED_DATE kullanılır (V1 lastUpdateDate karşılığı = variant.sellerModifiedDate).
public enum EnumApprovedDateQueryType
{
    VARIANT_CREATED_DATE,
    VARIANT_MODIFIED_DATE,
    CONTENT_MODIFIED_DATE
}
