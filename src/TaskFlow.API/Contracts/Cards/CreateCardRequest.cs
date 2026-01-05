using TaskFlow.Domain.Enums;

namespace TaskFlow.API.Contracts.Cards;

public sealed class CreateCardRequest
{
    public Guid ListId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TaskPriority Priority { get; init; } = TaskPriority.Medium;

    public int Position { get; init; }

    public DateTime? DueDateUtc { get; init; }
}
