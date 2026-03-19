using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration.Models.Response;

public class GetQuestionsFilterResponseModel : PaginationModel
{
    public List<GetQuestionResponseModel> Content { get; set; } = new();
}

public class GetQuestionResponseModel : IResponseModel
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string Text { get; set; } = null!;
    public string? UserName { get; set; }
    public bool ShowUserName { get; set; }
    public string? ProductName { get; set; }
    public string? ProductMainId { get; set; }
    public string? ImageUrl { get; set; }
    public string? WebUrl { get; set; }
    public string Status { get; set; } = null!;
    public bool Public { get; set; }
    public long CreationDate { get; set; }
    public string? AnsweredDateMessage { get; set; }
    public string? Reason { get; set; }
    public string? ReportReason { get; set; }
    public long? RejectedDate { get; set; }
    public long? ReportedDate { get; set; }
    public GetQuestionAnswerModel? Answer { get; set; }
    public GetQuestionRejectedAnswerModel? RejectedAnswer { get; set; }

    /// <summary>
    /// Integration layer tarafından stamp edilir (TrendyolQnAIntegration.GetQuestionsAsync).
    /// </summary>
    public int TY_SUPPLIERID { get; set; }
}

public class GetQuestionAnswerModel : IResponseModel
{
    public long Id { get; set; }
    public long CreationDate { get; set; }
    public bool HasPrivateInfo { get; set; }
    public string? Reason { get; set; }
    public string Text { get; set; } = null!;
}

public class GetQuestionRejectedAnswerModel : IResponseModel
{
    public long Id { get; set; }
    public long CreationDate { get; set; }
    public string? Reason { get; set; }
    public string Text { get; set; } = null!;
}
