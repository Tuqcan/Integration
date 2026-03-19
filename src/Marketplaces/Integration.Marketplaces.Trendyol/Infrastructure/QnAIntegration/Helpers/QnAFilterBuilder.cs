using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration.Helpers;

public class QnAFilterBuilder : IFilterBuilder
{
    private readonly Dictionary<string, string> _parameters = new();

    public QnAFilterBuilder AddPage(int page)
    {
        _parameters["page"] = page.ToString();
        return this;
    }

    public QnAFilterBuilder AddSize(int size)
    {
        if (size > 50)
            throw new ArgumentException("QnA page size must be less than or equal to 50");
        _parameters["size"] = size.ToString();
        return this;
    }

    public QnAFilterBuilder AddOrderByDirection(string direction)
    {
        _parameters["orderByDirection"] = direction;
        return this;
    }

    /// <summary>
    /// Belirtilen tarihten sonraki soruları getirir. Timestamp(millisecond).
    /// </summary>
    public QnAFilterBuilder AddStartDate(long unixMs)
    {
        _parameters["startDate"] = unixMs.ToString();
        return this;
    }

    /// <summary>
    /// Belirtilen tarihe kadar olan soruları getirir. Timestamp(millisecond).
    /// </summary>
    public QnAFilterBuilder AddEndDate(long unixMs)
    {
        _parameters["endDate"] = unixMs.ToString();
        return this;
    }

    /// <summary>
    /// Soruları statüye göre filtreler.
    /// Değerler: WAITING_FOR_ANSWER, ANSWERED, REPORTED, REJECTED, UNANSWERED
    /// </summary>
    public QnAFilterBuilder AddStatus(string status)
    {
        _parameters["status"] = status;
        return this;
    }

    /// <summary>
    /// Sıralama alanı. LastModifiedDate veya CreatedDate.
    /// </summary>
    public QnAFilterBuilder AddOrderByField(string field)
    {
        _parameters["orderByField"] = field;
        return this;
    }

    public string Build()
    {
        return string.Join("&", _parameters.Select(p => $"{p.Key}={p.Value}"));
    }
}
