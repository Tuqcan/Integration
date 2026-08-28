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

    /// <summary>
    /// Icerik seviyesindeki ve varyant seviyesindeki ozellikleri tek listeye birlestirir.
    ///
    /// CAKISMADA VARYANT KAZANIR: bir attributeId varyant seviyesinde de geliyorsa
    /// ayirt edici bilgi varyanta ozgudur; icerik seviyesindeki deger tum varyantlar
    /// icin ortak olan/varsayilan degerdir ve DUSURULUR.
    ///
    /// Birlestirme ID BAZLI, ad bazli DEGIL: V1'de "Renk" (47, serbest metin) ve
    /// "Web Color" (348, listeden secim) gibi AYNI ADA FARKLI ID ciftleri vardi ve
    /// ikisi de listede duruyordu. Ad bazli birlestirme bunlardan birini sessizce yerdi.
    ///
    /// ############ COK DEGERLI OZELLIK KORUNUR ############
    /// Onceki surum <c>Dictionary&lt;int, ...&gt;</c> kullaniyordu, yani attributeId basina
    /// TEK satir tutuyordu. Bir ozellik ayni urunde birden fazla degerle gelebilir
    /// (<c>allowMultipleAttributeValues = true</c>; ornek: "Uyumlu Marka" 37 secenek) -
    /// o durumda SONUNCUSU HARIC hepsi sessizce dusuyordu.
    ///
    /// Bunun varsayim olmadiginin kaniti tuketici tarafinda: ProductConsumer'in dedup
    /// anahtari <c>(AttributeId, AttributeValueId, CustomValue)</c> - yani sistem ozellik
    /// basina COKLU SATIR bekliyor. Sozluk o beklentiyi kiriyordu.
    ///
    /// Bugun ulasilabilir DEGIL (28.08.2026 canli olcumu: 190 leaf kategori / 4.151
    /// attribute satirinda <c>allowMultipleAttributeValues=true</c> HIC gorulmedi), ama
    /// dogru davranis bedava: artik ID'ye gore EZME yapiliyor, TEKILLEME degil.
    /// ####################################################
    /// </summary>
    private static List<FilterProductAttributeResponseModel> MergeAttributes(
        List<FilterProductAttributeResponseModel>? contentAttributes,
        List<FilterProductAttributeResponseModel>? variantAttributes)
    {
        if (variantAttributes == null || variantAttributes.Count == 0)
            return contentAttributes ?? [];

        // Varyantin konustugu attributeId'ler icerik tarafini EZER - ama YALNIZCA
        // o ID'ler icin. Diger ID'lerin coklu degerleri aynen korunur.
        var overriddenIds = variantAttributes.Select(a => a.AttributeId).ToHashSet();

        var merged = new List<FilterProductAttributeResponseModel>();

        foreach (var attribute in contentAttributes ?? [])
        {
            if (!overriddenIds.Contains(attribute.AttributeId))
                merged.Add(attribute);
        }

        merged.AddRange(variantAttributes);

        return merged;
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
                    Origin = variant.Origin,

                    Images = content.Images ?? [],

                    // Icerik + varyant ozellikleri BIRLESTIRILIR. V1'de liste duz
                    // geliyordu; V2 onu iki seviyeye boldu ve yalnizca content'i
                    // okumak varyanta ozgu ozellikleri (Beden vb.) dusururdu.
                    Attributes = MergeAttributes(content.Attributes, variant.Attributes),

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
                Origin = item.Origin,

                Images = item.Images ?? [],
                Attributes = item.Attributes ?? [],
            });
        }

        return result;
    }
}
