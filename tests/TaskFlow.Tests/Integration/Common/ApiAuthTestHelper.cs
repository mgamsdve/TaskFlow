using System.Net.Http.Headers;
using System.Net.Http.Json;
using TaskFlow.API.Contracts.Auth;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Integration.Common;

public static class ApiAuthTestHelper
{
    public static async Task<AuthResponse> RegisterAsync(
        HttpClient client,
        UserRole role = UserRole.Owner,
        string? email = null,
        string password = "StrongPass!123")
    {
        var request = new RegisterRequest
        {
            FullName = "Integration User",
            Email = email ?? $"user-{Guid.NewGuid():N}@taskflow.test",
            Password = password,
            Role = role
        };

        var response = await client.PostAsJsonAsync("/auth/register", request);
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return payload ?? throw new InvalidOperationException("Register response payload is null.");
    }

    public static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
