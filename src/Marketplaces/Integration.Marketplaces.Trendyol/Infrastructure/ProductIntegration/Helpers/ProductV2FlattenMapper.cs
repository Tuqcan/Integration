using System.Security.Cryptography;
using System.Text;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;

// V2 nested response'lari, mevcut flat FilterProductResponseModel'e duzlestirir.
// Boylece bus mesaji + consumer + DB sozlesmesi degismez.
//
// KIMLIK (ProductId): V1 'id' alani = MD5(supplierId + "_" + barcode) (gercek prod verisiyle kanitlandi).
// V2 bu hash'i dondurmedigi icin ayni formulle uretilir -> mevcut DB satirlari ayni id ile UPDATE olur.
//
// MALIYET KORUMASI: approved endpoint DimensionalWeight (desi), StockUnitType, PlatformListingId,
// HasActiveCampaign dondurmez. Bunlar burada default birakilir; consumer UPDATE'te mevcut degeri korur
// (overwrite etmez) -> kargo maliyeti/kar bozulmaz.
public static class ProductV2FlattenMapper
{
    public static string BuildProductId(int supplierId, string barcode)
    {
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"{supplierId}_{barcode}"));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public static List<FilterProductResponseModel> FlattenApproved(ApprovedProductsV2Response? response)
    {
        var result = new List<FilterProductResponseModel>();
        if (response?.Content == null) return result;

        foreach (var content in response.Content)
        {
            if (content.Variants == null) continue;

            foreach (var variant in content.Variants)
            {
                if (string.IsNullOrWhiteSpace(variant.Barcode)) continue;

                result.Add(new FilterProductResponseModel
                {
                    Id = BuildProductId(variant.SupplierId, variant.Barcode),
                    SupplierId = variant.SupplierId,

                    Approved = true,
                    Rejected = false,
                    Archived = variant.Archived,
                    Blacklisted = variant.Blacklisted,
                    Locked = variant.Locked,
                    Onsale = variant.OnSale,

                    // V1 createDateTime/lastUpdateDate = variant.seller*Date (content.lastModifiedDate DEGIL)
                    CreateDateTime = variant.SellerCreatedDate,
                    LastUpdateDate = variant.SellerModifiedDate,

                    Brand = content.Brand?.Name,
                    BrandId = content.Brand?.Id ?? 0,
                    CategoryName = content.Category?.Name,
                    PimCategoryId = content.Category?.Id ?? 0,

                    Title = content.Title,
                    Description = content.Description,
                    ProductMainId = content.ProductMainId,
                    ProductContentId = content.ContentId,
                    ProductCode = variant.VariantId,

                    Barcode = variant.Barcode,
                    StockCode = variant.StockCode,
                    Quantity = variant.Stock?.Quantity ?? 0,
                    ListPrice = variant.Price?.ListPrice ?? 0,
                    SalePrice = variant.Price?.SalePrice ?? 0,
                    VatRate = variant.VatRate ?? 0,
                    ProductUrl = variant.ProductUrl,
                    DeliveryDuration = variant.DeliveryOptions?.DeliveryDuration,

                    Images = content.Images ?? [],
                    Attributes = content.Attributes ?? [],

                    // DimensionalWeight / StockUnitType / PlatformListingId / HasActiveCampaign:
                    // V2 vermiyor -> default. Consumer UPDATE'te mevcut degeri korur.
                });
            }
        }

        return result;
    }

    public static List<FilterProductResponseModel> FlattenUnapproved(UnapprovedProductsV2Response? response)
    {
        var result = new List<FilterProductResponseModel>();
        if (response?.Content == null) return result;

        foreach (var item in response.Content)
        {
            if (string.IsNullOrWhiteSpace(item.Barcode)) continue;

            result.Add(new FilterProductResponseModel
            {
                Id = BuildProductId(item.SupplierId, item.Barcode),
                SupplierId = item.SupplierId,

                Approved = false,
                Rejected = string.Equals(item.Status, "rejected", StringComparison.OrdinalIgnoreCase),
                Archived = false,
                Blacklisted = false,
                Locked = false,
                Onsale = false,

                CreateDateTime = item.CreateDateTime,
                LastUpdateDate = item.LastUpdateDate,

                Brand = item.Brand?.Name,
                BrandId = item.Brand?.Id ?? 0,
                CategoryName = item.Category?.Name,
                PimCategoryId = item.Category?.Id ?? 0,

                Title = item.Title,
                Description = item.Description,
                ProductMainId = item.ProductMainId,

                Barcode = item.Barcode,
                StockCode = item.StockCode,
                Quantity = item.Quantity ?? 0,
                ListPrice = item.ListPrice ?? 0,
                SalePrice = item.SalePrice ?? 0,
                VatRate = item.VatRate ?? 0,
                DimensionalWeight = item.DimensionalWeight ?? 0,

                Images = item.Images ?? [],
                Attributes = item.Attributes ?? [],
            });
        }

        return result;
    }
}
