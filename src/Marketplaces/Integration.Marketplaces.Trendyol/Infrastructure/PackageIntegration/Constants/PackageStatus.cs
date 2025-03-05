namespace Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;
public enum PackageStatus
{
    Awaiting,
    Created,
    Picking,
    Invoiced,
    Shipped,
    AtCollectionPoint,
    Cancelled,
    Delivered,
    UnDelivered,
    UnDeliveredAndReturned,
    Returned,
    Repack,
    UnPacked,
    UnSupplied,
    Verified
}