using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

namespace Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration;

public class TrendyolQnAIntegration : TrendyolIntegrationBase, ITrendyolQnAIntegration
{
    public TrendyolQnAIntegration(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm, IRateLimiter? rateLimiter = null)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, rateLimiter) { }

    public async Task<GetQuestionsFilterResponseModel?> GetQuestionsAsync(string filterQuery)
    {
        string url = $"{GetBaseUrl()}qna/sellers/{SupplierId}/questions/filter" +
                     (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        var response = await GetAsync<GetQuestionsFilterResponseModel>(url, TrendyolRateLimitCategories.QnAFilter);

        foreach (var item in response?.Content ?? new List<GetQuestionResponseModel>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }

        return response;
    }
}
