using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Request;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;
using Integration.Marketplaces.Trendyol.Models.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;

public class TrendyolProductIntegration : TrendyolIntegrationBase, ITrendyolProductIntegration
{
    public TrendyolProductIntegration(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm, IRateLimiter? rateLimiter = null)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, rateLimiter) { }

    // ✅ API Endpoint Metodları
    // ############################################################################
    // YAZMA YOLU PATH'LERI - 2026-08 V2 DUZELTMESI
    //
    // Asagidaki uclarin HICBIRI su an cagrilmiyor (kutuphane paylasimli oldugu
    // icin silinmiyor, duzeltiliyor). Eski "suppliers/{id}/..." onekinin ZATEN
    // OLU oldugu canli dogrulandi: batch ucu eski path'te HTTP 556, yeni path'te
    // 200 donuyor (kanit: developer-md/trendyol-v2-fixtures/BATCH_OLD.json,
    // BATCH_NEW.json).
    //
    // ⚠️ Path'ler dokumantasyona gore duzeltildi; CANLI ISTEKLE DOGRULANMADI
    //    (uretim magazasina urun yazamayiz). Kullanmadan once stage'de test edin.
    // ############################################################################
    private string GetCreateProductsUrl() => $"{GetBaseUrl()}product/sellers/{SupplierId}/v2/products";
    private string GetSupplierAddressUrl() => $"{GetBaseUrl()}sellers/{SupplierId}/addresses";
    private string GetBrandsUrl() => $"{GetBaseUrl()}product/brands";
    private string GetCategoryTreeUrl() => $"{GetBaseUrl()}product/product-categories";
    // V2 (15.09.2026'da V1 kapaniyor). DIKKAT: kategori AGACI (GetCategoryTreeUrl) hala
    // 'product/product-categories' - o uc V1-V2 ortak ve canlida 200 donuyor, DEGISTIRILMEZ.
    private string GetCategoryAttributesUrl(int categoryId) => $"{GetBaseUrl()}product/categories/{categoryId}/attributes";
    private string GetCategoryAttributeValuesUrl(int categoryId, int attributeId, int page, int size)
        => $"{GetBaseUrl()}product/categories/{categoryId}/attributes/{attributeId}/values?page={page}&size={size}";
    private string GetUpdateProductUrl() => $"{GetBaseUrl()}product/sellers/{SupplierId}/v2/products";
    // DIKKAT: bu ucun oneki "inventory/", "product/" DEGIL.
    private string GetUpdatePriceAndStockUrl() => $"{GetBaseUrl()}inventory/sellers/{SupplierId}/products/price-and-inventory";
    private string GetDeleteProductUrl() => $"{GetBaseUrl()}product/sellers/{SupplierId}/products";
    private string GetBatchRequestResultUrl(string batchRequestId) => $"{GetBaseUrl()}product/sellers/{SupplierId}/products/batch-requests/{batchRequestId}";
    private string GetFilterApprovedProductsUrl() => $"{GetBaseUrl()}product/sellers/{SupplierId}/products/approved";
    private string GetFilterUnapprovedProductsUrl() => $"{GetBaseUrl()}product/sellers/{SupplierId}/products/unapproved";

    // ⚠️ Tedarikçi Adreslerini Getir - path V2'ye guncellendi (2026-08), CANLI DOGRULANMADI.
    [Obsolete("Path V2'ye guncellendi (sellers/{id}/addresses) ama canli dogrulanmadi. " +
              "Bu uc siparis servis grubuna ait; siparis modulu gecisinde ele alinacak.")]
    public async Task<GetSuppliersAddressesResponseModel?> GetSuppliersAddressesAsync()
    {
        return await GetAsync<GetSuppliersAddressesResponseModel>(GetSupplierAddressUrl(), TrendyolRateLimitCategories.SupplierAddresses);
    }

    // ✅ Kargo Şirketlerini Getir
    public List<GetProviderResponseModel> GetProviders()
    {
        return Providers.GetProviders();
    }

    // ✅ Markaları Getir
    public async Task<GetBrandsResponseModel?> GetBrandsAsync()
    {
        return await GetAsync<GetBrandsResponseModel>(GetBrandsUrl(), TrendyolRateLimitCategories.Brands);
    }

    // ✅ Kategori Ağacını Getir
    public async Task<GetCategoryTreeResponseModel?> GetCategoryTreeAsync()
    {
        return await GetAsync<GetCategoryTreeResponseModel>(GetCategoryTreeUrl(), TrendyolRateLimitCategories.Categories);
    }

    // ✅ Kategoriye Ait Özellikleri Getir
    public async Task<GetCategoryAttributesResponseModel?> GetCategoryAttributes(int categoryId)
    {
        return await GetAsync<GetCategoryAttributesResponseModel>(GetCategoryAttributesUrl(categoryId), TrendyolRateLimitCategories.CategoryAttributes);
    }

    /// <summary>
    /// Kategori-ozellik DEGERLERINI getirir (V2 - ayri, sayfali uc).
    ///
    /// V1'de degerler GetCategoryAttributes yanitinin icinde geliyordu; V2 o alani kaldirdi (§1.1).
    /// Deger kumesi kategoriden BAGIMSIZ (canli kanit §1.2 + 28.08 tekrar olcumu: attribute 1192
    /// icin kategori 766/5511/384 birebir ayni 230 degeri donuyor) -> cagiran taraf attribute
    /// basina TEK kategori ile cekebilir.
    ///
    /// ############ SAYFA BOYUTU: 1000 DEGIL ############
    /// Canli olcum (28.08.2026, attribute 292 = 2.140 deger):
    ///   size=1000 -> content 1000, totalPages 3   (3 istek)
    ///   size=2000 -> content 2000, totalPages 2   (2 istek)
    ///   size=3000 -> content 2140, totalPages 1   (1 ISTEK)
    /// Yani <c>size</c> 1000'in USTUNDE de onurlandiriliyor; 1000 varsayilani gereksiz yere
    /// istek sayisini 3 katina cikariyordu. Varsayilan <see cref="DefaultValuePageSize"/>.
    ///
    /// Cagiran taraf sayfalamayi YINE DE dogru yapmali: Trendyol ileride size'i tavanlarsa
    /// yanit <c>size</c>/<c>totalPages</c> alanlarini kucuk degerlerle doner ve dongu
    /// kendiliginden coklu sayfaya doner (CategoriesWorker.FetchAttributeValuePagesAsync).
    ///
    /// ⚠️ ARALIK DISI SAYFA ISTEMEYIN: canli olcumde <c>page=9999</c> -> HTTP 500,
    ///    <c>page=-1</c> -> HTTP 500. Dongu totalElements/dolu-sayfa olcutuyle durmali.
    /// #################################################
    /// </summary>
    public async Task<GetCategoryAttributeValuesResponseModel?> GetCategoryAttributeValuesAsync(
        int categoryId, int attributeId, int page = 0, int size = DefaultValuePageSize, CancellationToken ct = default)
    {
        return await GetAsync<GetCategoryAttributeValuesResponseModel>(
            GetCategoryAttributeValuesUrl(categoryId, attributeId, page, size),
            TrendyolRateLimitCategories.CategoryAttributeValues, ct);
    }

    /// <summary>
    /// Deger ucu icin varsayilan sayfa boyutu.
    ///
    /// DUZELTME (28.08.2026): burada "canli olculen en buyuk deger kumesi 2.140
    /// (attribute 292)" yaziyordu — o olcum yalnizca bir kac ozelligi ornekliyordu.
    /// Deger tasiyan 965 ozelligin TAMAMI tarandi:
    ///     attribute 344 "Yazar" -> 156.200 deger = 53 sayfa
    ///     ikinci en buyuk       ->   2.140 deger =  1 sayfa
    /// Yani 2.140 gercekten IKINCI en buyuktu; birincisi 73 kat buyuk.
    ///
    /// 3000, ozelliklerin 964/965'ini TEK sayfada getiriyor. Daha buyuk bir varsayilan
    /// yalnizca "Yazar" icin fark yaratir ve o kume zaten sayfalanmak zorunda; sayfa
    /// dongusunun kacak tavani CategoriesWorker.MaxAttributeValuePages'te.
    /// </summary>
    public const int DefaultValuePageSize = 3000;

    /// <summary>
    /// ⚠️ Path V2'ye guncellendi (2026-08): product/sellers/{id}/v2/products.
    /// CANLI ISTEKLE DOGRULANMADI - kullanmadan once stage'de test edin.
    /// </summary>
    public async Task<bool> CreateProductsV2Async(BulkModel<CreateProductRequestModel> products)
    {
        return await PostAsync<BulkModel<CreateProductRequestModel>, bool>(GetCreateProductsUrl(), products, TrendyolRateLimitCategories.ProductCreate);
    }

    /// <summary>
    /// ⚠️ V2'de updateProduct TEK UC OLMAKTAN CIKTI, dorde bolundu:
    /// products/unapproved-bulk-update · products/content-bulk-update ·
    /// products/variant-bulk-update · products/delivery-info-bulk-update
    ///
    /// Bu metot yalnizca ONEKI duzeltilmis haliyle duruyor (davranis DEGISMEDI);
    /// gercek V2 karsiligi dort ayri cagri gerektirir. CANLI DOGRULANMADI.
    /// </summary>
    [Obsolete("V2'de 4 uca bolundu: unapproved-bulk-update / content-bulk-update / " +
              "variant-bulk-update / delivery-info-bulk-update. Bu metot canli dogrulanmadi.")]
    public async Task<bool> UpdateProductAsync(BulkModel<UpdateProductRequestModel> products)
    {
        return await PutAsync<BulkModel<UpdateProductRequestModel>, bool>(GetUpdateProductUrl(), products, TrendyolRateLimitCategories.ProductUpdate);
    }

    /// <summary>
    /// Stok ve fiyat guncelleme.
    ///
    /// ⚠️ Path V2'ye guncellendi (2026-08): inventory/sellers/{id}/products/price-and-inventory
    /// (onek "inventory/", "product/" DEGIL). CANLI ISTEKLE DOGRULANMADI.
    ///
    /// Rate limit: 14.09.2026 oncesi bu uc KOVASIZ (limitsiz) cagriliyordu.
    /// Yeni rejimde kendi kovasi var (@T50K 350/dk); kovasiz birakmak 429 uretirdi.
    /// </summary>
    public async Task<bool> UpdatePriceAndInventoryAsync(BulkModel<UpdateStockAndPriceRequestModel> products)
    {
        return await PutAsync<BulkModel<UpdateStockAndPriceRequestModel>, bool>(
            GetUpdatePriceAndStockUrl(), products, TrendyolRateLimitCategories.StockPriceWrite);
    }

    /// <summary>
    /// ⚠️ Path VE HTTP METODU duzeltildi (2026-08):
    /// eski: PUT suppliers/{id}/v2/products  ->  yeni: DELETE product/sellers/{id}/products
    ///
    /// Eski hali YANLIS METOTLA yanlis uca gidiyordu; iki hatanin birbirini
    /// gizlemesi sayesinde patlamamisti (cagiran yok). CANLI DOGRULANMADI.
    /// </summary>
    public async Task<bool> DeleteProductsAsync(BulkModel<DeleteProductRequestModel> products)
    {
        return await DeleteAsync<BulkModel<DeleteProductRequestModel>, bool>(
            GetDeleteProductUrl(), products, TrendyolRateLimitCategories.ProductDelete);
    }

    /// <summary>
    /// ⚠️ Path V2'ye guncellendi (2026-08): product/sellers/{id}/products/batch-requests/{b}.
    /// Eski path'in ZATEN OLU oldugu canli dogrulandi (556 vs 200) - bkz.
    /// trendyol-v2-fixtures/BATCH_OLD.json / BATCH_NEW.json.
    /// </summary>
    public async Task<GetBatchRequestResultResponseModel> GetBatchRequestResultAsync(string batchRequestId)
    {
        return await GetAsync<GetBatchRequestResultResponseModel>(GetBatchRequestResultUrl(batchRequestId), TrendyolRateLimitCategories.BatchCheck);
    }

    // ✅ Onaylı Ürünleri Getir (V2). Nested V2 response flat modele düzleştirilir; bus/consumer değişmez.
    public async Task<FilterProductsResponseModel?> FilterApprovedProductsAsync(string filterQuery)
    {
        string url = GetFilterApprovedProductsUrl() + (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var v2 = await GetAsync<ApprovedProductsV2Response>(url, TrendyolRateLimitCategories.ProductFilter);
        var flat = ProductV2FlattenMapper.FlattenApproved(v2);
        foreach (var item in flat)
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);

        return new FilterProductsResponseModel
        {
            Content = flat,
            TotalElements = v2?.TotalElements ?? 0,
            TotalPages = v2?.TotalPages ?? 0,
            Page = v2?.Page ?? 0,
            Size = v2?.Size ?? 0,
            NextPageToken = v2?.NextPageToken
        };
    }

    // ✅ Onaysız Ürünleri Getir (V2 - pending/rejected). Flat modele düzleştirilir.
    public async Task<FilterProductsResponseModel?> FilterUnapprovedProductsAsync(string filterQuery)
    {
        string url = GetFilterUnapprovedProductsUrl() + (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var v2 = await GetAsync<UnapprovedProductsV2Response>(url, TrendyolRateLimitCategories.ProductFilter);
        var flat = ProductV2FlattenMapper.FlattenUnapproved(v2);
        foreach (var item in flat)
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);

        return new FilterProductsResponseModel
        {
            Content = flat,
            TotalElements = v2?.TotalElements ?? 0,
            TotalPages = v2?.TotalPages ?? 0,
            Page = v2?.Page ?? 0,
            Size = v2?.Size ?? 0,
            NextPageToken = v2?.NextPageToken
        };
    }
}
