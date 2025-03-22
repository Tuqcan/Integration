using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.Marketplaces.Trendyol
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMarketplacesTrendyol(this IServiceCollection services)
        {

            services.AddHttpClient("TrendyolApi", client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            //services.AddScoped<ITrendyolPackageIntegration, TrendyolPackageIntegration>();
            //services.AddScoped<ITrendyolProductIntegration, TrendyolProductIntegration>();
            //services.AddScoped<ITrendyolClaimIntegration, TrendyolClaimIntegration>();

            services.AddSingleton<TrendyolIntegrationFactory>();
            return services;
        }
    }
}
