using System.Net.Http.Json;
using System.Text.Json;
using TaskFlow.Blazor.Models;

namespace TaskFlow.Blazor.Services;

public sealed class AuthService
{
    private const string TokenStorageKey = "taskflow.auth.token";
    private const string UserStorageKey = "taskflow.auth.user";

    private readonly HttpClient _httpClient;
    private readonly LocalStorageService _localStorageService;
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private bool _initialized;

    public AuthService(HttpClient httpClient, LocalStorageService localStorageService)
    {
        _httpClient = httpClient;
        _localStorageService = localStorageService;
    }

    public UserDto? CurrentUser { get; private set; }

    public string? Token { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public event Action? AuthStateChanged;

    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }

        var token = await _localStorageService.GetItemAsync(TokenStorageKey);
        var userJson = await _localStorageService.GetItemAsync(UserStorageKey);

        Token = string.IsNullOrWhiteSpace(token) ? null : token;

        if (!string.IsNullOrWhiteSpace(userJson))
        {
            CurrentUser = JsonSerializer.Deserialize<UserDto>(userJson, _serializerOptions);
        }

        _initialized = true;
        AuthStateChanged?.Invoke();
    }

    public async Task<string?> GetTokenAsync()
    {
        await InitializeAsync();
        return Token;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", request, cancellationToken);
        var payload = await ReadPayloadAsync(response, cancellationToken);

        await SetSessionAsync(payload);
        return payload;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/register", request, cancellationToken);
        var payload = await ReadPayloadAsync(response, cancellationToken);

        await SetSessionAsync(payload);
        return payload;
    }

    public async Task LogoutAsync()
    {
        Token = null;
        CurrentUser = null;

        await _localStorageService.RemoveItemAsync(TokenStorageKey);
        await _localStorageService.RemoveItemAsync(UserStorageKey);

        AuthStateChanged?.Invoke();
    }

    private async Task SetSessionAsync(AuthResponse response)
    {
        Token = response.Token;
        CurrentUser = response.User;

        await _localStorageService.SetItemAsync(TokenStorageKey, Token);
        await _localStorageService.SetItemAsync(UserStorageKey, JsonSerializer.Serialize(CurrentUser, _serializerOptions));

        AuthStateChanged?.Invoke();
    }

    private async Task<AuthResponse> ReadPayloadAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var message = string.IsNullOrWhiteSpace(content)
                ? $"Request failed with status code {(int)response.StatusCode}."
                : content;

            throw new InvalidOperationException(message);
        }

        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>(_serializerOptions, cancellationToken);
        return payload ?? throw new InvalidOperationException("API response was empty.");
    }
}
