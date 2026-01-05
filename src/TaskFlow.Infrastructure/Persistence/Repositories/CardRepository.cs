using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class CardRepository : ICardRepository
{
    private readonly AppDbContext _dbContext;

    public CardRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Card?> GetByIdAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        if (cardId == Guid.Empty)
        {
            return null;
        }

        return await _dbContext.Cards
            .AsSplitQuery()
            .Include(card => card.Comments)
                .ThenInclude(comment => comment.Author)
            .Include(card => card.Labels)
            .Include(card => card.AssignedUsers)
            .FirstOrDefaultAsync(card => card.Id == cardId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Card>> GetByListIdAsync(Guid listId, CancellationToken cancellationToken = default)
    {
        if (listId == Guid.Empty)
        {
            return [];
        }

        var cards = await _dbContext.Cards
            .AsSplitQuery()
            .Where(card => card.ListId == listId)
            .OrderBy(card => card.Position)
            .Include(card => card.Comments)
                .ThenInclude(comment => comment.Author)
            .Include(card => card.Labels)
            .Include(card => card.AssignedUsers)
            .ToListAsync(cancellationToken);

        return cards;
    }

    public async Task AddAsync(Card card, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(card);

        await _dbContext.Cards.AddAsync(card, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Card card, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(card);

        _dbContext.Cards.Update(card);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
