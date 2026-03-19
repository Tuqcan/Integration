using System;
using Microsoft.Extensions.DependencyInjection;
using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.OrderIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration;
using Integration.Marketplaces.Trendyol.Infrastructure.QnAIntegration;

namespace Integration.Marketplaces.Trendyol.Infrastructure;

public class TrendyolIntegrationFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRateLimiter? _rateLimiter;

    public TrendyolIntegrationFactory(IHttpClientFactory httpClientFactory, IRateLimiter? rateLimiter = null)
    {
        _httpClientFactory = httpClientFactory;
        _rateLimiter = rateLimiter;
    }

    public T Create<T>(string supplierId, string apiKey, string apiSecret, bool isInProduction, string entegratorFirm) where T : class
    {
        if (typeof(T) == typeof(ITrendyolPackageIntegration))
        {
            return new TrendyolPackageIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, _rateLimiter) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolPackageIntegration");
        }

        if (typeof(T) == typeof(ITrendyolProductIntegration))
        {
            return new TrendyolProductIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, _rateLimiter) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolProductIntegration");
        }

        if (typeof(T) == typeof(ITrendyolClaimIntegration))
        {
            return new TrendyolClaimIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, _rateLimiter) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolClaimIntegration");
        }

        if (typeof(T) == typeof(ITrendyolInvoiceIntegration))
        {
            return new TrendyolInvoiceIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, _rateLimiter) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolInvoiceIntegration");
        }

        if (typeof(T) == typeof(ITrendyolQnAIntegration))
        {
            return new TrendyolQnAIntegration(_httpClientFactory, supplierId, apiKey, apiSecret, isInProduction, entegratorFirm, _rateLimiter) as T
                ?? throw new InvalidOperationException("Failed to create TrendyolQnAIntegration");
        }

        throw new NotImplementedException($"Integration type {typeof(T).Name} is not supported.");
    }
}
