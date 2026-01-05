using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
