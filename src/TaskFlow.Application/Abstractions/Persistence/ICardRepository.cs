using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions.Persistence;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid cardId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Card>> GetByListIdAsync(Guid listId, CancellationToken cancellationToken = default);

    Task AddAsync(Card card, CancellationToken cancellationToken = default);

    Task UpdateAsync(Card card, CancellationToken cancellationToken = default);
}
