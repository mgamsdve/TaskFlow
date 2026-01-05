using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs;

public sealed class RegisterUserRequest
{
    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public UserRole Role { get; init; } = UserRole.Member;
}
