using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskFlow.Blazor.Services;

public abstract class ApiServiceBase
{
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    protected ApiServiceBase(HttpClient httpClient, AuthService authService)
    {
        HttpClient = httpClient;
        AuthService = authService;
    }

    protected HttpClient HttpClient { get; }

    protected AuthService AuthService { get; }

    protected async Task<T?> SendAsync<T>(HttpMethod method, string uri, object? payload = null, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(method, uri, payload);
        using var response = await HttpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await AuthService.LogoutAsync();
        }

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("Access denied. Your account role does not have permission for this action.");
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                ? $"Request to '{uri}' failed with status code {(int)response.StatusCode}."
                : error);
        }

        if (response.Content.Headers.ContentLength is 0)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(_serializerOptions, cancellationToken);
    }

    protected async Task SendAsync(HttpMethod method, string uri, object? payload = null, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(method, uri, payload);
        using var response = await HttpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await AuthService.LogoutAsync();
        }

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new InvalidOperationException("Access denied. Your account role does not have permission for this action.");
            }

            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(error)
                ? $"Request to '{uri}' failed with status code {(int)response.StatusCode}."
                : error);
        }
    }

    private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string uri, object? payload)
    {
        var request = new HttpRequestMessage(method, uri);

        var token = await AuthService.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        if (payload is not null)
        {
            request.Content = JsonContent.Create(payload);
        }

        return request;
    }
}
