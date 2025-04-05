using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.JsonConverter;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.Models.Response;

public class GetInvoiceCargoResponseModel: PaginationModel
{
    public List<GetInvoiceCargoContent>? Content { get; set; }  
}

public class GetInvoiceCargoContent
{
    public string ShipmentPackageType { get; set; } = null!; // Gönderi Kargo Bedeli / İade Kargo Bedeli
    public long ParcelUniqueId { get; set; }
    public string OrderNumber { get; set; } = null!;         
    public decimal Amount { get; set; }             
    public int Desi { get; set; }

    public int TY_SUPPLIERID { get; set; }
    public Guid UserId { get; set; } = Guid.Empty;
}