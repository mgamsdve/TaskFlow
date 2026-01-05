using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Boards;

namespace TaskFlow.Application.Abstractions.Services;

public interface IBoardService
{
    Task<BoardDto?> GetBoardAsync(GetBoardQuery query, CancellationToken cancellationToken = default);
}
