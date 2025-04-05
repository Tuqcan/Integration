using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.Helpers;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Helpers;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;

var services = new ServiceCollection();
services.AddHttpClient(); // IHttpClientFactory'yi ekler

using var provider = services.BuildServiceProvider();
using var scope = provider.CreateScope();

var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();


//CLAIM
var trendyolClaimIntegration = new TrendyolClaimIntegration(
        httpClientFactory: clientFactory,
                supplierId: "193500",
            apiKey: "1pQRqfqycYJYy3duv1xn",
            apiSecret: "g8Sodg5HVhPRsOS8NVkD",
            isInProduction: true,
            entegratorFirm: "entegratorFirm");

var ClaimFilter = new ClaimFilterBuilder().AddSize(200);
await trendyolClaimIntegration.GetClaimsAsync(ClaimFilter.Build());







var trendyolOrderIntegration = new TrendyolPackageIntegration(
        httpClientFactory: clientFactory,
                supplierId: "193500",
            apiKey: "1pQRqfqycYJYy3duv1xn",
            apiSecret: "g8Sodg5HVhPRsOS8NVkD",
            isInProduction: true,
            entegratorFirm: "entegratorFirm");


var orderfilter = new ShipmentFilterBuilder().AddStatus(Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration.Constants.PackageStatus.Created)
    .Build();

var orders = await trendyolOrderIntegration.GetShipmentPackagesAsync(orderfilter);

//var test = "";

//var trendyolProductIntegration = new TrendyolProductIntegration(
//        supplierId: "193500",
//        apiKey: "1pQRqfqycYJYy3duv1xn",
//        apiSecret: "g8Sodg5HVhPRsOS8NVkD",
//        isInProduction: true,
//        entegratorFirm: "entegratorFirm");

//try
//{
//    //Get All Categories
//    var categories = await trendyolProductIntegration.GetCategoryTreeAsync();
//}
//catch (Exception ex)
//{ 
//}


////Get All Brands
//var brands = await trendyolProductIntegration.GetBrandsAsync();

////Filter products
var productFilter = new ProductFilterBuilder()
    .AddApprovalStatus(true)
    .AddBarcode("w56ıe56thw5e6hw56")
    .AddStartDate(0)
    .AddEndDate(0)
    .AddPage(1)
    .AddSize(10)
    .AddSupplierId(0)
    .Build();

//var products = await trendyolProductIntegration.FilterProductsAsync(productFilter);

//