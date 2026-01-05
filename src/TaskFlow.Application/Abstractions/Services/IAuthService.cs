using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Abstractions.Services;

public interface IAuthService
{
    Task<UserDto> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default);
}
