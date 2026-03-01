using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.FinanceIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.RateLimiting;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration;

public class TrendyolInvoiceIntegration : TrendyolIntegrationBase, ITrendyolInvoiceIntegration
{
    public TrendyolInvoiceIntegration(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm, IRateLimiter? rateLimiter = null)
        : base(httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, rateLimiter) { }


    public async Task<GetFinancialTransactionsResponseModel?> GetInvoices(string filterQuery)
    {
        string url = $"{GetBaseUrl()}finance/che/sellers/{SupplierId}/settlements" +
                     (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        return await GetAsync<GetFinancialTransactionsResponseModel>(url, TrendyolRateLimitCategories.InvoiceSettlements);
    }

    public async Task<GetOtherFinancialTransactionsResponseModel?> GetOtherInvoices(string filterQuery)
    {
        string url = $"{GetBaseUrl()}finance/che/sellers/{SupplierId}/otherfinancials" +
                    (string.IsNullOrWhiteSpace(filterQuery) ? "" : "?" + filterQuery);

        return await GetAsync<GetOtherFinancialTransactionsResponseModel>(url, TrendyolRateLimitCategories.InvoiceSettlements);
    }


    public async Task<GetInvoiceCargoResponseModel?> GetCargo(string invoiceSerialNumber, int page = 0, int pageSize = 500)
    {
        string url = $"{GetBaseUrl()}finance/che/sellers/{SupplierId}/cargo-invoice/{invoiceSerialNumber}/items?size={pageSize}&page={page}";

        var response = await GetAsync<GetInvoiceCargoResponseModel>(url, TrendyolRateLimitCategories.InvoiceCargo);
        foreach (var item in response?.Content ?? new List<GetInvoiceCargoContent>())
        {
            item.TY_SUPPLIERID = Convert.ToInt32(SupplierId);
        }

        return response;
    }
}
