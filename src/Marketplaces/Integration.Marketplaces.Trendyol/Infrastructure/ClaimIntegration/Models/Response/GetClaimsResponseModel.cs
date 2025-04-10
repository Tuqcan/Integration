using Integration.Hub;
namespace Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.Models.Response;
public class GetClaimsResponseModel : PaginationModel
{
    public List<GetClaimResponseModel> Content { get; set; }
}

public class GetClaimResponseModel : IResponseModel
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; }
    public long OrderDate { get; set; }
    public string CustomerFirstName { get; set; }
    public string CustomerLastName { get; set; }
    public long ClaimDate { get; set; }
    public long? CargoTrackingNumber { get; set; }
    public string? CargoTrackingLink { get; set; }
    public string? CargoSenderNumber { get; set; }
    public string? CargoProviderName { get; set; }
    public long OrderShipmentPackageId { get; set; }
    public GetClaimReplacementOutboundPackageInfoResponseModel? ReplacementOutboundPackageInfo { get; set; }
    public GetClaimRejectedPackageInfoResponseModel? RejectedPackageInfo { get; set; }
    public List<GetClaimClaimItemResponseModel> Items { get; set; }

    public long? LastModifiedDate { get; set; }
    public long? OrderOutboundPackageId { get; set; }

    public int TY_SUPPLIERID { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;
}

public class GetClaimReplacementOutboundPackageInfoResponseModel : IResponseModel
{
    public long? CargoTrackingNumber { get; set; }
    public string? CargoProviderName { get; set; }
    public string? CargoSenderNumber { get; set; }
    public string? CargoTrackingLink { get; set; }
    public long Packageid { get; set; }
    public List<string> Items { get; set; }
}

public class GetClaimRejectedPackageInfoResponseModel : IResponseModel
{
    public long? CargoTrackingNumber { get; set; }
    public string? CargoProviderName { get; set; }
    public string? CargoTrackingLink { get; set; }
    public string? CargoSenderNumber { get; set; }
    public int Packageid { get; set; }
    public List<string> Items { get; set; }
}

public class GetClaimClaimItemResponseModel : IResponseModel
{
    public GetClaimClaimOrderLineResponseModel OrderLine { get; set; }
    public List<GetClaimClaimItemItemResponseModel> ClaimItems { get; set; }
}

public class GetClaimClaimOrderLineResponseModel : IResponseModel
{
    public long Id { get; set; }
    public string ProductName { get; set; }
    public string Barcode { get; set; }
    public string MerchantSku { get; set; }
    public string ProductColor { get; set; }
    public string ProductSize { get; set; }
    public decimal Price { get; set; }
    public decimal VatBaseAmount { get; set; }
    public int SalesCampaignId { get; set; }
    public string ProductCategory { get; set; }
}

public class GetClaimClaimItemItemResponseModel : IResponseModel
{
    public Guid Id { get; set; }
    public long OrderLineItemId { get; set; }
    public GetClaimClaimItemReasonResponseModel CustomerClaimItemReason { get; set; }
    public GetClaimClaimItemReasonResponseModel TrendyolClaimItemReason { get; set; }
    public GetClaimClaimItemStatusResponseModel ClaimItemStatus { get; set; }
    public string Note { get; set; }
    public string CustomerNote { get; set; }
    public bool? Resolved { get; set; }
    public bool? AutoAccepted { get; set; }
    public bool? AcceptedBySeller { get; set; }
}

public class GetClaimClaimItemReasonResponseModel : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ExternalReasonId { get; set; }
    public string Code { get; set; }
}

public class GetClaimClaimItemStatusResponseModel : IResponseModel
{
    public string Name { get; set; }
}