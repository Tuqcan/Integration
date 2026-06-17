using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;

// GET .../products/unapproved (V2) için query builder.
// size <= 1000, dateQueryType = EnumdateQueryType (CREATED_DATE/LAST_MODIFIED_DATE), token-based sayfalama.
public class UnapprovedProductFilterBuilder : IFilterBuilder
{
    private const int MaxSize = 1000;
    private readonly Dictionary<string, string> _parameters = new();

    public UnapprovedProductFilterBuilder AddSize(int size)
    {
        _parameters["size"] = Math.Min(size, MaxSize).ToString();
        return this;
    }

    public UnapprovedProductFilterBuilder AddStartDate(long startDate)
    {
        _parameters["startDate"] = startDate.ToString();
        return this;
    }

    public UnapprovedProductFilterBuilder AddEndDate(long endDate)
    {
        _parameters["endDate"] = endDate.ToString();
        return this;
    }

    public UnapprovedProductFilterBuilder AddDateQueryType(EnumdateQueryType dateQueryType)
    {
        _parameters["dateQueryType"] = dateQueryType.ToString();
        return this;
    }

    public UnapprovedProductFilterBuilder AddNextPageToken(string nextPageToken)
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
