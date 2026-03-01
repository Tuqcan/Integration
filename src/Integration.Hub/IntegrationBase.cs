using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Integration.Hub;
public abstract class IntegrationBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    protected readonly string SupplierId;
    protected readonly string ApiKey;
    protected readonly string ApiSecret;
    protected readonly JsonSerializerOptions _jsonOptions;
    protected readonly IRateLimiter? _rateLimiter;

    private const int MaxTransientRetries = 3;

    protected IntegrationBase(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret, IRateLimiter? rateLimiter = null)
    {
        _httpClientFactory = httpClientFactory;
        SupplierId = supplierId;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        _rateLimiter = rateLimiter;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    protected HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("TrendyolApi");
        var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ApiKey}:{ApiSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        AddHeaders(client);

        return client;
    }

    protected virtual void AddHeaders(HttpClient client) { }

    #region Rate-Limited HTTP Methods

    protected async Task<TResponse> GetAsync<TResponse>(string url, string? rateLimitCategory = null)
    {
        int transientRetries = 0;

        while (true)
        {
            if (rateLimitCategory != null && _rateLimiter != null)
                await _rateLimiter.WaitAsync(rateLimitCategory);

            using var client = CreateClient();
            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            }
            catch (HttpRequestException) when (transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            if (response.StatusCode == (HttpStatusCode)429)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                await Task.Delay(retryAfter);
                continue;
            }

            if (IsTransientError(response.StatusCode) && transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            return await HandleResponse<TResponse>(response);
        }
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request, string? rateLimitCategory = null)
    {
        int transientRetries = 0;

        while (true)
        {
            if (rateLimitCategory != null && _rateLimiter != null)
                await _rateLimiter.WaitAsync(rateLimitCategory);

            using var client = CreateClient();
            var jsonData = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(url, requestBody);
            }
            catch (HttpRequestException) when (transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            if (response.StatusCode == (HttpStatusCode)429)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                await Task.Delay(retryAfter);
                continue;
            }

            if (IsTransientError(response.StatusCode) && transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            return await HandleResponse<TResponse>(response);
        }
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest request, string? rateLimitCategory = null)
    {
        int transientRetries = 0;

        while (true)
        {
            if (rateLimitCategory != null && _rateLimiter != null)
                await _rateLimiter.WaitAsync(rateLimitCategory);

            using var client = CreateClient();
            var jsonData = JsonSerializer.Serialize(request, _jsonOptions);
            var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.PutAsync(url, requestBody);
            }
            catch (HttpRequestException) when (transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            if (response.StatusCode == (HttpStatusCode)429)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                await Task.Delay(retryAfter);
                continue;
            }

            if (IsTransientError(response.StatusCode) && transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            return await HandleResponse<TResponse>(response);
        }
    }

    protected async Task<bool> DeleteAsync(string url, string? rateLimitCategory = null)
    {
        int transientRetries = 0;

        while (true)
        {
            if (rateLimitCategory != null && _rateLimiter != null)
                await _rateLimiter.WaitAsync(rateLimitCategory);

            using var client = CreateClient();
            HttpResponseMessage response;
            try
            {
                response = await client.DeleteAsync(url);
            }
            catch (HttpRequestException) when (transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            if (response.StatusCode == (HttpStatusCode)429)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
                await Task.Delay(retryAfter);
                continue;
            }

            if (IsTransientError(response.StatusCode) && transientRetries < MaxTransientRetries)
            {
                transientRetries++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, transientRetries)));
                continue;
            }

            return response.IsSuccessStatusCode;
        }
    }

    #endregion

    private static bool IsTransientError(HttpStatusCode code)
        => code is HttpStatusCode.InternalServerError
            or HttpStatusCode.BadGateway
            or HttpStatusCode.ServiceUnavailable
            or HttpStatusCode.GatewayTimeout;

    private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response)
    {
        try
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException($"Trendyol API erisim hatasi. StatusCode: {(int)response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception(
                    $"API istegi basarisiz oldu. StatusCode: {(int)response.StatusCode} - {response.StatusCode}. " +
                    $"Response: {errorContent}");
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TResponse>(responseStream, _jsonOptions)
                   ?? throw new JsonException("JSON donusumu basarisiz.");
        }
        catch (OutOfMemoryException ex)
        {
            throw new OutOfMemoryException("Bellek asimi olustu! JSON verisi cok buyuk olabilir.", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException("JSON hatasi: Gecersiz format!", ex);
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException and not OutOfMemoryException and not JsonException)
        {
            throw new Exception($"Istek islenirken hata olustu: {ex.Message}", ex);
        }
    }
}
