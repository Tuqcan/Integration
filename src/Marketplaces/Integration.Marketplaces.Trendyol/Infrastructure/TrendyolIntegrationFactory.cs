using System;
using Microsoft.Extensions.DependencyInjection;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;

namespace Integration.Marketplaces.Trendyol.Infrastructure;

public class TrendyolIntegrationFactory
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TrendyolIntegrationFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public T Create<T>(string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm) where T : class
    {
        if (typeof(T) == typeof(ITrendyolPackageIntegration))
        {
            return new TrendyolPackageIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolPackageIntegration");
        }

        if (typeof(T) == typeof(ITrendyolProductIntegration))
        {
            return new TrendyolProductIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolProductIntegration");
        }

        if (typeof(T) == typeof(ITrendyolClaimIntegration))
        {
            return new TrendyolClaimIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolClaimIntegration");
        }

        throw new NotImplementedException($"Integration type {typeof(T).Name} is not supported.");
    }
}
