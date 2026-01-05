using TaskFlow.Domain.Enums;

namespace TaskFlow.API.Contracts.Cards;

public sealed class UpdateCardRequest
{
    public string? Title { get; init; }

    public string? Description { get; init; }

    public bool ClearDescription { get; init; }

    public TaskPriority? Priority { get; init; }

    public DateTime? DueDateUtc { get; init; }

    public bool ClearDueDate { get; init; }

    public int? Position { get; init; }

    public Guid? ListId { get; init; }
}
