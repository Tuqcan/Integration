using Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration.Models.Response;

namespace Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration;

public interface ITrendyolQnAIntegration
{
    /// <summary>
    /// Trendyol QnA sistemindeki müşteri sorularını filtreli olarak çeker.
    /// Tarih parametresi verilmezse son 1 hafta, verilirse max 2 haftalık aralık.
    /// </summary>
    public Task<GetQuestionsFilterResponseModel?> GetQuestionsAsync(string filterQuery);
}
