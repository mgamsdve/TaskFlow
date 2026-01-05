using TaskFlow.Domain.Enums;

namespace TaskFlow.API.Contracts.Auth;

public sealed class RegisterRequest
{
    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public UserRole Role { get; init; } = UserRole.Member;
}
