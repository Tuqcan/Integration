using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.Models.Request;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.Models.Response;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration;

public class TrendyolClaimIntegration : TrendyolIntegrationBase, ITrendyolClaimIntegration
{
    public TrendyolClaimIntegration(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm) { }

    public async Task<GetClaimsResponseModel?> GetClaimsAsync(string filterQuery)
    {
        // string url = $"{GetBaseUrl()}suppliers/{SupplierId}/claims" + (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        string url = $"{GetBaseUrl()}order/sellers/{SupplierId}/claims"+
             (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);
        var response = await GetAsync<GetClaimsResponseModel>(url);
        foreach (var item in response?.Content ?? new List<GetClaimResponseModel>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }

        return response;
    }

    public async Task<bool> CreateClaimAsync(CreateClaimRequestModel createClaimRequestModel)
    {
        return await PostAsync<CreateClaimRequestModel, bool>($"{GetBaseUrl()}suppliers/{SupplierId}/claims/create", createClaimRequestModel);
    }

    public async Task<bool> ApproveClaimLineItemsAsync(ApproveClaimLineItemsRequestModel approveClaimLineItemsRequestModel, string claimId)
    {
        return await PutAsync<ApproveClaimLineItemsRequestModel, bool>($"{GetBaseUrl()}claims/{claimId}/items/approve", approveClaimLineItemsRequestModel);
    }
}
