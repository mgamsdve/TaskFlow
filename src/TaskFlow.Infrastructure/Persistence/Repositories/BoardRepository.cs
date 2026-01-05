using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class BoardRepository : IBoardRepository
{
    private readonly AppDbContext _dbContext;

    public BoardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Board?> GetByIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        if (boardId == Guid.Empty)
        {
            return null;
        }

        return await _dbContext.Boards
            .AsSplitQuery()
            .Include(board => board.Lists)
                .ThenInclude(list => list.Cards)
                    .ThenInclude(card => card.Comments)
                        .ThenInclude(comment => comment.Author)
            .Include(board => board.Lists)
                .ThenInclude(list => list.Cards)
                    .ThenInclude(card => card.Labels)
            .Include(board => board.Lists)
                .ThenInclude(list => list.Cards)
                    .ThenInclude(card => card.AssignedUsers)
            .FirstOrDefaultAsync(board => board.Id == boardId, cancellationToken);
    }
}
