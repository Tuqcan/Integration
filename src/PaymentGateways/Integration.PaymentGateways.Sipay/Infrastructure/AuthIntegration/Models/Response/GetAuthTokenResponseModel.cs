using Integration.PaymentGateways.Sipay.Infrastructure.AuthIntegration.Models.Response;

namespace Integration.PaymentGateways.Sipay.Infrastructure.AuthIntegration;
public class GetAuthTokenResponseModel : SipayBaseResponseModel<GetAuthTokenResponseModel>
{
    public string StatusCode { get; set; }
    public bool Is3D { get; set; }
    public string ExpiresAt { get; set; }
}