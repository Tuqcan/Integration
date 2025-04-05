

using Integration.Marketplaces.Trendyol.Infrastructure.FinanceIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Models.Response;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration;

public interface ITrendyolInvoiceIntegration
{
    Task<GetFinancialTransactionsResponseModel?> GetInvoices(string filterQuery);
    Task<GetOtherFinancialTransactionsResponseModel?> GetOtherInvoices(string filterQuery);

    Task<GetInvoiceCargoResponseModel?> GetCargo(string invoiceSerialNumber, int page = 0, int pageSize = 500);
}
