namespace TaskFlow.Blazor.Models;

public sealed class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public UserDto User { get; set; } = new();
}
