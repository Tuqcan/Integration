using System.Net;
using System.Text;
using System.Text.Json;

namespace Integration.Hub;

//public class IntegrationBase
//{
//    private readonly IHttpClientFactory _httpClient;
//    private readonly JsonSerializerOptions _options;

//    public IntegrationBase(HttpClient httpClient)
//    {
//        _httpClient = httpClient;
//        _options = new JsonSerializerOptions
//        {
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//            WriteIndented = true
//        };
//    }

//    public void InitializeDefaultHeaders(Dictionary<string, string> headers)
//    {
//        _httpClient.DefaultRequestHeaders.Clear();
//        foreach (var header in headers)
//            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
//    }

//    public void SetHeader(KeyValuePair<string, string> header)
//    {
//        if (!_httpClient.DefaultRequestHeaders.Contains(header.Key))
//            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
//    }

//    public async Task<TResponse> InvokeRequestAsync<TResponse>(Func<HttpClient, Task<HttpResponseMessage>> httpRequest)
//        where TResponse : IResponseModel
//    {
//        var response = await httpRequest.Invoke(_httpClient);
//        var responseAsString = await response.Content.ReadAsStringAsync();

//        if (response.StatusCode == HttpStatusCode.Unauthorized)
//            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");

//        if (!response.IsSuccessStatusCode)
//            throw new HttpRequestException($"HTTP Hata Kodu: {response.StatusCode}, ��erik: {responseAsString}");

//        return JsonSerializer.Deserialize<TResponse>(responseAsString, _options)
//            ?? throw new Exception("Yan�t deserialization s�ras�nda null d�nd�!");
//    }

//    public async Task<bool> InvokeRequestAsync(Func<HttpClient, Task<HttpResponseMessage>> httpRequest)
//    {
//        var response = await httpRequest.Invoke(_httpClient);
//        var responseAsString = await response.Content.ReadAsStringAsync();

//        if (response.StatusCode == HttpStatusCode.Unauthorized)
//            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");

//        if (!response.IsSuccessStatusCode)
//            throw new HttpRequestException($"HTTP Hata Kodu: {response.StatusCode}, ��erik: {responseAsString}");

//        return response.IsSuccessStatusCode;
//    }

//    public async Task<TResponse> InvokeRequestAsync<TRequest, TResponse>(
//        Func<HttpClient, StringContent?, Task<HttpResponseMessage>> httpRequest, TRequest requestModel)
//        where TResponse : IResponseModel
//        where TRequest : IRequestModel
//    {
//        var jsonData = JsonSerializer.Serialize(requestModel, _options);
//        var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

//        var response = await httpRequest.Invoke(_httpClient, requestBody);
//        var responseAsString = await response.Content.ReadAsStringAsync();

//        if (response.StatusCode == HttpStatusCode.Unauthorized)
//            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");

//        if (!response.IsSuccessStatusCode)
//            throw new HttpRequestException($"HTTP Hata Kodu: {response.StatusCode}, ��erik: {responseAsString}");

//        return JsonSerializer.Deserialize<TResponse>(responseAsString, _options)
//            ?? throw new Exception("Yan�t deserialization s�ras�nda null d�nd�!");
//    }

//    public async Task<bool> InvokeRequestAsync<TRequest>(
//        Func<HttpClient, StringContent?, Task<HttpResponseMessage>> httpRequest, TRequest requestModel)
//        where TRequest : IRequestModel
//    {
//        var jsonData = JsonSerializer.Serialize(requestModel, _options);
//        var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

//        var response = await httpRequest.Invoke(_httpClient, requestBody);
//        var responseAsString = await response.Content.ReadAsStringAsync();

//        if (response.StatusCode == HttpStatusCode.Unauthorized)
//            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");

//        if (!response.IsSuccessStatusCode)
//            throw new HttpRequestException($"HTTP Hata Kodu: {response.StatusCode}, ��erik: {responseAsString}");

//        return response.IsSuccessStatusCode;
//    }
//}





public class IntegrationBase
{
    protected static readonly HttpClient _httpClient;
    protected static JsonSerializerOptions _options = default!;
    static IntegrationBase()
    {
        _httpClient = new HttpClient();
    }

    protected void InitializeDefaultHeaders(Dictionary<string, string> headers)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        foreach (var header in headers)
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
    }

    protected void SetHeader(KeyValuePair<string, string> header)
    {
        _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
    }

    protected void SetSerializerOptions(JsonSerializerOptions options)
    {
        _options = options;
    }

    public async Task<TResponse> InvokeRequestAsync<TResponse>(Func<HttpClient, Task<HttpResponseMessage>> httpRequest)
    where TResponse : IResponseModel
    {
        var response = await httpRequest.Invoke(_httpClient);
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");
        var isSuccess = response.IsSuccessStatusCode;
        if (!isSuccess)
            throw new Exception(responseAsString);

        return JsonSerializer.Deserialize<TResponse>(responseAsString, _options)!;
    }

    public async Task<bool> InvokeRequestAsync(Func<HttpClient, Task<HttpResponseMessage>> httpRequest)
    {
        var response = await httpRequest.Invoke(_httpClient);
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");
        var isSuccess = response.IsSuccessStatusCode;
        if (!isSuccess)
            throw new Exception(responseAsString);

        return isSuccess;
    }

    public async Task<TResponse> InvokeRequestAsync<TRequest, TResponse>(Func<HttpClient, StringContent?, Task<HttpResponseMessage>> httpRequest, TRequest requestModel)
    where TResponse : IResponseModel
    where TRequest : IRequestModel
    {
        var jsonData = JsonSerializer.Serialize(requestModel, _options);
        var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await httpRequest.Invoke(_httpClient, requestBody);
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");
        var isSuccess = response.IsSuccessStatusCode;
        if (!isSuccess)
            throw new Exception(responseAsString);
        return JsonSerializer.Deserialize<TResponse>(responseAsString, _options)!;
    }
    public async Task<bool> InvokeRequestAsync<TRequest>(Func<HttpClient, StringContent?, Task<HttpResponseMessage>> httpRequest, TRequest requestModel)
    where TRequest : IRequestModel
    {
        var jsonData = JsonSerializer.Serialize(requestModel, _options);
        var requestBody = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await httpRequest.Invoke(_httpClient, requestBody);
        var responseAsString = await response.Content.ReadAsStringAsync();
        var isSuccess = response.IsSuccessStatusCode;
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("Trendyol Api Bilgileri hatal�.");
        if (!isSuccess)
            throw new Exception(responseAsString);
        return isSuccess;
    }
}