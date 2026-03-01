using System.Net.Http;
using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure;
public abstract class TrendyolIntegrationBase : IntegrationBase
{
    protected readonly bool IsInProduction;
    protected readonly string EntegratorFirm;

    protected TrendyolIntegrationBase(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm, IRateLimiter? rateLimiter = null)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, rateLimiter)
    {
        IsInProduction = isInProduction;
        EntegratorFirm = entegratorFirm;
    }

    public string GetBaseUrl() => IsInProduction
        ? "https://apigw.trendyol.com/integration/"
        : "https://stageapigw.trendyol.com/integration/";

    protected override void AddHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("User-Agent", $"{EntegratorFirm}-{SupplierId}");
    }
}
