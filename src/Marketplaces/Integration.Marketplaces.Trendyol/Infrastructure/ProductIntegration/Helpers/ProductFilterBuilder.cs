using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;
public class ProductFilterBuilder : IFilterBuilder
{
    private readonly Dictionary<string, string> _parameters = new();

    public ProductFilterBuilder AddApprovalStatus(bool status)
    {
        _parameters["approvalStatus"] = status.ToString().ToLower(); // true/false
        return this;
    }

    public ProductFilterBuilder AddBarcode(string barcode)
    {
        _parameters["barcode"] = barcode;
        return this;
    }

    public ProductFilterBuilder AddStartDate(long startDate)
    {
        _parameters["startDate"] = startDate.ToString();
        return this;
    }

    public ProductFilterBuilder AddEndDate(long endDate)
    {
        _parameters["endDate"] = endDate.ToString();
        return this;
    }

    public ProductFilterBuilder AddPage(int page)
    {
        if (page > 2500)
            throw new Exception("Page must be less than 2500");
        _parameters["page"] = page.ToString();
        return this;
    }

    public ProductFilterBuilder AdddateQueryType(EnumdateQueryType dateQueryType)
    {
        _parameters["dateQueryType"] = dateQueryType.ToString();
        return this;
    }

    public ProductFilterBuilder AddSize(int size)
    {
        _parameters["size"] = size.ToString();
        return this;
    }

    public ProductFilterBuilder AddSupplierId(long supplierId)
    {
        _parameters["supplierId"] = supplierId.ToString();
        return this;
    }

    public ProductFilterBuilder AddStockCode(string stockCode)
    {
        _parameters["stockCode"] = stockCode;
        return this;
    }

    public ProductFilterBuilder AddArchived(bool archived)
    {
        _parameters["archived"] = archived.ToString().ToLower();
        return this;
    }

    public ProductFilterBuilder AddProductMainId(string productMainId)
    {
        _parameters["productMainId"] = productMainId;
        return this;
    }

    public ProductFilterBuilder AddOnSale(bool onSale)
    {
        _parameters["onSale"] = onSale.ToString().ToLower();
        return this;
    }

    public ProductFilterBuilder AddRejected(bool rejected)
    {
        _parameters["rejected"] = rejected.ToString().ToLower();
        return this;
    }

    public ProductFilterBuilder AddBlacklisted(bool blacklisted)
    {
        _parameters["blacklisted"] = blacklisted.ToString().ToLower();
        return this;
    }

    public ProductFilterBuilder AddBrandIds(List<int> brandIds)
    {
        _parameters["brandIds"] = string.Join(",", brandIds);
        return this;
    }

    public string Build()
    {
        return string.Join("&", _parameters.Select(p => $"{p.Key}={p.Value}"));
    }
}