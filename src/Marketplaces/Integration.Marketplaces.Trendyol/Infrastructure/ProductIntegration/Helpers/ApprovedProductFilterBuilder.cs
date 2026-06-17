using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;

// GET .../products/approved (V2) için query builder.
// size <= 100, dateQueryType = EnumApprovedDateQueryType, token-based sayfalama.
public class ApprovedProductFilterBuilder : IFilterBuilder
{
    private const int MaxSize = 100;
    private readonly Dictionary<string, string> _parameters = new();

    public ApprovedProductFilterBuilder AddSize(int size)
    {
        _parameters["size"] = Math.Min(size, MaxSize).ToString();
        return this;
    }

    public ApprovedProductFilterBuilder AddStartDate(long startDate)
    {
        _parameters["startDate"] = startDate.ToString();
        return this;
    }

    public ApprovedProductFilterBuilder AddEndDate(long endDate)
    {
        _parameters["endDate"] = endDate.ToString();
        return this;
    }

    public ApprovedProductFilterBuilder AddDateQueryType(EnumApprovedDateQueryType dateQueryType)
    {
        _parameters["dateQueryType"] = dateQueryType.ToString();
        return this;
    }

    public ApprovedProductFilterBuilder AddNextPageToken(string nextPageToken)
    {
        // Token base64 (+, /, =) içerebilir -> URL-encode şart.
        _parameters["nextPageToken"] = Uri.EscapeDataString(nextPageToken);
        return this;
    }

    public string Build()
    {
        return string.Join("&", _parameters.Select(p => $"{p.Key}={p.Value}"));
    }
}
