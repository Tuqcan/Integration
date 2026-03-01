using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Request;
using Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration;

public class TrendyolPackageIntegration : TrendyolIntegrationBase, ITrendyolPackageIntegration
{
    public TrendyolPackageIntegration(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm, IRateLimiter? rateLimiter = null)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, rateLimiter) { }

    public async Task<GetShipmentPackagesResponseModel?> GetShipmentPackagesAsync(string filterQuery)
    {
        string url = $"{GetBaseUrl()}order/sellers/{SupplierId}/orders" +
                     (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var response = await GetAsync<GetShipmentPackagesResponseModel>(url, TrendyolRateLimitCategories.ShipmentPackages);
        foreach (var item in response?.Content ?? new List<GetShipmentPackagePackageResponseModel>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }

        return response;
    }

    public async Task<bool> UpdateTrackingNumberAsync(long shipmentPackageId, UpdateTrackingNumberRequestModel updateTrackingNumberRequestModel)
    {
        return await PutAsync<UpdateTrackingNumberRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/{shipmentPackageId}/update-tracking-number", updateTrackingNumberRequestModel, TrendyolRateLimitCategories.TrackingNumber);
    }

    public async Task<bool> UpdatePackageAsync(long shipmentPackageId, UpdatePackageRequestModel updatePackageRequestModel)
    {
        return await PutAsync<UpdatePackageRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/shipment-packages/{shipmentPackageId}", updatePackageRequestModel, TrendyolRateLimitCategories.PackageStatus);
    }

    public async Task<bool> SendInvoiceLinkAsync(AddInvoiceLinkRequestModel addInvoiceLinkRequestModel)
    {
        return await PostAsync<AddInvoiceLinkRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/supplier-invoice-links", addInvoiceLinkRequestModel, TrendyolRateLimitCategories.PackageStatus);
    }

    public async Task<bool> DeleteInvoiceLinkAsync(DeleteInvoiceLinkRequestModel deleteInvoiceLinkRequestModel)
    {
        return await PostAsync<DeleteInvoiceLinkRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/supplier-invoice-links/delete", deleteInvoiceLinkRequestModel, TrendyolRateLimitCategories.PackageStatus);
    }

    public async Task<bool> SplitMultiPackageByQuantityAsync(long shipmentPackageId, SplitMultiPackageByQuantityRequestModel splitMultiPackageByQuantityRequestModel)
    {
        return await PostAsync<SplitMultiPackageByQuantityRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/shipment-packages/{shipmentPackageId}/split-packages", splitMultiPackageByQuantityRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    public async Task<bool> SplitMultiShipmentPackageAsync(long shipmentPackageId, SplitMultiShipmentPackageRequestModel splitMultiShipmentPackageRequestModel)
    {
        return await PostAsync<SplitMultiShipmentPackageRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/shipment-packages/{shipmentPackageId}/split", splitMultiShipmentPackageRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    public async Task<bool> SplitShipmentPackageAsync(long shipmentPackageId, SplitShipmentPackageRequestModel splitShipmentPackageRequestModel)
    {
        return await PostAsync<SplitShipmentPackageRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/shipment-packages/{shipmentPackageId}/multi-split", splitShipmentPackageRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    public async Task<bool> SplitShipmentPackageByQuantityAsync(long shipmentPackageId, SplitMultiPackageByQuantityRequestModel splitMultiPackageByQuantityRequestModel)
    {
        return await PostAsync<SplitMultiPackageByQuantityRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/shipment-packages/{shipmentPackageId}/quantity-split", splitMultiPackageByQuantityRequestModel, TrendyolRateLimitCategories.SplitPackages);
    }

    public async Task<bool> UpdateBoxInfoAsync(long shipmentPackageId, UpdateBoxInfoRequestModel updateBoxInfoRequestModel)
    {
        return await PostAsync<UpdateBoxInfoRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/shipment-packages/{shipmentPackageId}/box-info", updateBoxInfoRequestModel, TrendyolRateLimitCategories.BoxInfo);
    }
}
