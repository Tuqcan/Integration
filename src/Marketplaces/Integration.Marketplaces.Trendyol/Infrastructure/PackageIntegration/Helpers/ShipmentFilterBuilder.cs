using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;

namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Helpers;
public class ShipmentFilterBuilder : IFilterBuilder
{
    private readonly Dictionary<string, string> _parameters = new();

    public ShipmentFilterBuilder AddStartDate(long startDate)
    {
        _parameters["startDate"] = startDate.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddEndDate(long endDate)
    {
        _parameters["endDate"] = endDate.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddPage(int page)
    {
        _parameters["page"] = page.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddSize(int size)
    {
        if (size > 10000)
            throw new Exception("Page size must be less than or equal to 10000");
        _parameters["size"] = size.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddSupplierId(long supplierId)
    {
        _parameters["supplierId"] = supplierId.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddOrderNumber(string orderNumber)
    {
        _parameters["orderNumber"] = orderNumber;
        return this;
    }

    public ShipmentFilterBuilder AddStatus(PackageStatus status)
    {
        _parameters["status"] = status.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddOrderByField(OrderField orderByField)
    {
        _parameters["orderByField"] = orderByField.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddOrderByDirection(OrderByDirection orderByDirection)
    {
        _parameters["orderByDirection"] = orderByDirection.ToString();
        return this;
    }

    public ShipmentFilterBuilder AddShipmentPackageIds(List<long> shipmentPackageIds)
    {
        _parameters["shipmentPackageIds"] = string.Join(",", shipmentPackageIds);
        return this;
    }

    public string Build()
    {
        return string.Join("&", _parameters.Select(p => $"{p.Key}={p.Value}"));
    }
}