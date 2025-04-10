using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants;
using System.Text.Json.Serialization;
namespace Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Response;
public class GetShipmentPackagesResponseModel : PaginationModel
{
    public List<GetShipmentPackagePackageResponseModel> Content { get; set; }

}
public class GetShipmentPackagePackageFastDeliveryOptionResponseModel : IResponseModel
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DeliveryOption Type { get; set; }
}
public class GetShipmentPackagePackageDiscountDetailResponseModel : IResponseModel
{
    public decimal LineItemPrice { get; set; }
    public decimal LineItemDiscount { get; set; }
    public decimal LineItemTyDiscount { get; set; }
}
public class GetShipmentPackagePackageLineResponseModel : IResponseModel
{
    public int Quantity { get; set; }
    public int SalesCampaignId { get; set; }
    public string ProductSize { get; set; }
    public string MerchantSku { get; set; }
    public string ProductName { get; set; }
    public int ProductCode { get; set; }
    public string ProductOrigin { get; set; }
    public int MerchantId { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public decimal TyDiscount { get; set; }
    public List<GetShipmentPackagePackageDiscountDetailResponseModel> DiscountDetails { get; set; }
    public List<GetShipmentPackagePackageFastDeliveryOptionResponseModel> FastDeliveryOptions { get; set; }
    public string CurrencyCode { get; set; }
    public string ProductColor { get; set; }
    public long Id { get; set; }
    public string Sku { get; set; }
    public decimal VatBaseAmount { get; set; }
    public string Barcode { get; set; }
    public string OrderLineItemStatusName { get; set; }
    public decimal Price { get; set; }
}
public class GetShipmentPackageShipmentAddressResponseModel : IResponseModel
{
    public long? Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public int CityCode { get; set; }
    public string District { get; set; }
    public int DistrictId { get; set; }
    public string PostalCode { get; set; }
    public string CountryCode { get; set; }
    public int NeighborhoodId { get; set; }
    public string Neighborhood { get; set; }
    public object Phone { get; set; }
    public string FullName { get; set; }
    public string FullAddress { get; set; }
}
public class GetShipmentPackageInvoiceAddressResponseModel : IResponseModel
{
    public long? Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public int DistrictId { get; set; }
    public string PostalCode { get; set; }
    public string CountryCode { get; set; }
    public int NeighborhoodId { get; set; }
    public string Neighborhood { get; set; }
    public object Phone { get; set; }
    public string FullName { get; set; }
    public string FullAddress { get; set; }
    public string TaxOffice { get; set; }
    public string TaxNumber { get; set; }
}
public class GetShipmentPackagesPackageHistoryResponseModel : IResponseModel
{
    public long CreatedDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PackageStatus Status { get; set; }
}
public class GetShipmentPackagePackageResponseModel : IResponseModel
{
    public GetShipmentPackageShipmentAddressResponseModel? ShipmentAddress { get; set; }
    public string OrderNumber { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTyDiscount { get; set; }
    public object TaxNumber { get; set; }
    public GetShipmentPackageInvoiceAddressResponseModel? InvoiceAddress { get; set; }
    public string CustomerFirstName { get; set; }
    public string CustomerEmail { get; set; }
    public int CustomerId { get; set; }
    public string CustomerLastName { get; set; }
    public long Id { get; set; }
    public long CargoTrackingNumber { get; set; }
    public string CargoTrackingLink { get; set; }
    public string CargoSenderNumber { get; set; }
    public string CargoProviderName { get; set; }
    public List<GetShipmentPackagePackageLineResponseModel> Lines { get; set; }
    public long OrderDate { get; set; }
    public string TcIdentityNumber { get; set; }
    public string CurrencyCode { get; set; }
    public List<GetShipmentPackagesPackageHistoryResponseModel> PackageHistories { get; set; }
    public string ShipmentPackageStatus { get; set; }
    public string Status { get; set; }
    public string DeliveryType { get; set; }
    public int TimeSlotId { get; set; }
    public string ScheduledDeliveryStoreId { get; set; }
    public long EstimatedDeliveryStartDate { get; set; }
    public long EstimatedDeliveryEndDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string DeliveryAddressType { get; set; }
    public long AgreedDeliveryDate { get; set; }
    public bool AgreedDeliveryDateExtendible { get; set; }
    public long ExtendedAgreedDeliveryDate { get; set; }
    public long AgreedDeliveryExtensionStartDate { get; set; }
    public long AgreedDeliveryExtensionEndDate { get; set; }
    public string InvoiceLink { get; set; }
    public bool FastDelivery { get; set; }
    public string? FastDeliveryType { get; set; }
    public long OriginShipmentDate { get; set; }
    public long LastModifiedDate { get; set; }
    public bool Commercial { get; set; }
    public bool DeliveredByService { get; set; }
    public bool Micro { get; set; }
    public string? EtgbNo { get; set; }
    public long? EtgbDate { get; set; }
    public bool GiftBoxRequested { get; set; }
    public int TY_SUPPLIERID { get; set; }
}