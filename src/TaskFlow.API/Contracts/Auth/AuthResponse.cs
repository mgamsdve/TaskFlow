using TaskFlow.Application.DTOs;

namespace TaskFlow.API.Contracts.Auth;

public sealed class AuthResponse
{
    public required string Token { get; init; }

    public required UserDto User { get; init; }
}
