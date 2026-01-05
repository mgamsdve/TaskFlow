using System.Net.Http.Json;
using TaskFlow.API.Contracts.Auth;
using TaskFlow.Domain.Enums;
using TaskFlow.Tests.Integration.Common;

namespace TaskFlow.Tests.Integration.Endpoints;

public sealed class AuthEndpointsTests : IClassFixture<TaskFlowApiFactory>
{
    private readonly TaskFlowApiFactory _factory;

    public AuthEndpointsTests(TaskFlowApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturnJwtToken()
    {
        using var client = _factory.CreateClient();

        var email = $"auth-{Guid.NewGuid():N}@taskflow.test";
        const string password = "StrongPass!123";

        var registerResponse = await client.PostAsJsonAsync(
            "/auth/register",
            new RegisterRequest
            {
                FullName = "Auth Test User",
                Email = email,
                Password = password,
                Role = UserRole.Owner
            });

        registerResponse.EnsureSuccessStatusCode();

        var registerPayload = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(registerPayload);
        Assert.False(string.IsNullOrWhiteSpace(registerPayload!.Token));
        Assert.Equal(email, registerPayload.User.Email);

        var loginResponse = await client.PostAsJsonAsync(
            "/auth/login",
            new LoginRequest
            {
                Email = email,
                Password = password
            });

        loginResponse.EnsureSuccessStatusCode();

        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(loginPayload);
        Assert.False(string.IsNullOrWhiteSpace(loginPayload!.Token));
        Assert.Equal(email, loginPayload.User.Email);
    }
}
