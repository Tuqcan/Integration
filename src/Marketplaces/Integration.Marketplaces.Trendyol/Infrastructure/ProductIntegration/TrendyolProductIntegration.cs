using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;
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
    private string GetCreateProductsUrl() => $"{GetBaseUrl()}suppliers/{SupplierId}/v2/products";
    private string GetSupplierAddressUrl() => $"{GetBaseUrl()}suppliers/{SupplierId}/addresses";
    private string GetBrandsUrl() => $"{GetBaseUrl()}product/brands";
    private string GetCategoryTreeUrl() => $"{GetBaseUrl()}product/product-categories";
    private string GetCategoryAttributesUrl(int categoryId) => $"{GetBaseUrl()}product/product-categories/{categoryId}/attributes";
    private string GetUpdateProductUrl() => $"{GetBaseUrl()}suppliers/{SupplierId}/v2/products";
    private string GetUpdatePriceAndStockUrl() => $"{GetBaseUrl()}suppliers/{SupplierId}/products/price-and-inventory";
    private string GetDeleteProductUrl() => $"{GetBaseUrl()}suppliers/{SupplierId}/v2/products";
    private string GetBatchRequestResultUrl(string batchRequestId) => $"{GetBaseUrl()}suppliers/{SupplierId}/products/batch-requests/{batchRequestId}";
    private string GetFilterProductsUrl() => $"{GetBaseUrl()}product/sellers/{SupplierId}/products";

    // ✅ Tedarikçi Adreslerini Getir
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

    // ✅ Yeni Ürün Oluştur (V2)
    public async Task<bool> CreateProductsV2Async(BulkModel<CreateProductRequestModel> products)
    {
        return await PostAsync<BulkModel<CreateProductRequestModel>, bool>(GetCreateProductsUrl(), products, TrendyolRateLimitCategories.ProductCreate);
    }

    // ✅ Ürün Güncelle
    public async Task<bool> UpdateProductAsync(BulkModel<UpdateProductRequestModel> products)
    {
        return await PutAsync<BulkModel<UpdateProductRequestModel>, bool>(GetUpdateProductUrl(), products, TrendyolRateLimitCategories.ProductUpdate);
    }

    // ✅ Stok ve Fiyat Güncelle (NO LIMIT)
    public async Task<bool> UpdatePriceAndInventoryAsync(BulkModel<UpdateStockAndPriceRequestModel> products)
    {
        return await PutAsync<BulkModel<UpdateStockAndPriceRequestModel>, bool>(GetUpdatePriceAndStockUrl(), products);
    }

    // ✅ Ürün Sil
    public async Task<bool> DeleteProductsAsync(BulkModel<DeleteProductRequestModel> products)
    {
        return await PutAsync<BulkModel<DeleteProductRequestModel>, bool>(GetDeleteProductUrl(), products, TrendyolRateLimitCategories.ProductDelete);
    }

    // ✅ Batch Request Sonucunu Getir
    public async Task<GetBatchRequestResultResponseModel> GetBatchRequestResultAsync(string batchRequestId)
    {
        return await GetAsync<GetBatchRequestResultResponseModel>(GetBatchRequestResultUrl(batchRequestId), TrendyolRateLimitCategories.BatchCheck);
    }

    // ✅ Ürünleri Filtreleyerek Getir
    public async Task<FilterProductsResponseModel?> FilterProductsAsync(string filterQuery)
    {
        string url = GetFilterProductsUrl() + (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var response = await GetAsync<FilterProductsResponseModel>(url, TrendyolRateLimitCategories.ProductFilter);
        foreach (var item in response?.Content ?? new List<FilterProductResponseModel>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }
        return response;
    }
}
