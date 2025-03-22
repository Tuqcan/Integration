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

    protected IntegrationBase(IHttpClientFactory httpClientFactory, string supplierId, string apiKey, string apiSecret)
    {
        _httpClientFactory = httpClientFactory;
        SupplierId = supplierId;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
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

        // Ekstra ba�l�klar� eklemek i�in �a��r�l�r
        AddHeaders(client);

        return client;
    }

    /// <summary>
    /// �ocuk s�n�flar�n ek ba�l�klar ekleyebilmesi i�in override edilebilir metod.
    /// </summary>
    protected virtual void AddHeaders(HttpClient client) { }

    protected async Task<TResponse> GetAsync<TResponse>(string url)
    {
        using var client = CreateClient();
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        return await HandleResponse<TResponse>(response);
    }

    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request)
    {
        using var client = CreateClient();
        var jsonData = JsonSerializer.Serialize(request, _jsonOptions);
        var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, requestBody);
        return await HandleResponse<TResponse>(response);
    }

    protected async Task<TResponse> PutAsync<TRequest, TResponse>(string url, TRequest request)
    {
        using var client = CreateClient();
        var jsonData = JsonSerializer.Serialize(request, _jsonOptions);
        var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await client.PutAsync(url, requestBody);
        return await HandleResponse<TResponse>(response);
    }

    protected async Task<bool> DeleteAsync(string url)
    {
        using var client = CreateClient();
        var response = await client.DeleteAsync(url);
        return response.IsSuccessStatusCode;
    }

    private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response)
    {
        try
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Trendyol API bilgileri hatal�.");

            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(); // **STREAM �LE OKUMA**
            return await JsonSerializer.DeserializeAsync<TResponse>(responseStream, _jsonOptions)
                   ?? throw new JsonException("JSON d�n���m� ba�ar�s�z.");
        }
        catch (OutOfMemoryException ex)
        {
            throw new OutOfMemoryException("Bellek a��m� olu�tu! JSON verisi �ok b�y�k olabilir.", ex);
        }
        catch (JsonException ex)
        {
            throw new JsonException("JSON d�n���m hatas�: Ge�ersiz format!", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"�stek i�lenirken hata olu�tu: {ex.Message}", ex);
        }
    }
}
