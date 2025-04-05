using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Constants;
using System.Text;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Helpers;

public class InvoiceFilterBuilder : IFilterBuilder
{
    private readonly StringBuilder _filterQuery;

    public InvoiceFilterBuilder()
    {
        _filterQuery = new StringBuilder();
    }

    public InvoiceFilterBuilder AddStartDate(long startDate)
    {
        _filterQuery.Append($"startDate={startDate}&");
        return this;
    }

    public InvoiceFilterBuilder AddEndDate(long endDate)
    {
        _filterQuery.Append($"endDate={endDate}&");
        return this;
    }

    public InvoiceFilterBuilder AddTransactionType(EnumTransactionType transactionType)
    {
        _filterQuery.Append($"transactionType={transactionType.ToString()}&");
        return this;
    }
    public InvoiceFilterBuilder AddTransactionOtherType(EnumOtherFinancialTransactionType transactionType)
    {
        _filterQuery.Append($"transactionType={transactionType.ToString()}&");
        return this;
    }

    public InvoiceFilterBuilder AddPage(int page)
    {
        _filterQuery.Append($"page={page}&");
        return this;
    }

    public InvoiceFilterBuilder AddSize(int size)
    {
        if (size > 1000)
            throw new ArgumentException("Size must be less than or equal to 1000");
        _filterQuery.Append($"size={size}&");
        return this;
    }

    public InvoiceFilterBuilder AddSupplierId(long supplierId)
    {
        _filterQuery.Append($"supplierId={supplierId}&");
        return this;
    }

    public string Build()
    {
        if (_filterQuery.Length == 0)
            return string.Empty;

        return _filterQuery.ToString().TrimEnd('&');
    }
}
