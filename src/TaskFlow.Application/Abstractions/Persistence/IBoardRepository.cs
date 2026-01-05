using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions.Persistence;

public interface IBoardRepository
{
    Task<Board?> GetByIdAsync(Guid boardId, CancellationToken cancellationToken = default);
}
