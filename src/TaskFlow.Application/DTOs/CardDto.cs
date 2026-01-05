using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.DTOs;

public sealed class CardDto
{
    public Guid Id { get; init; }

    public Guid ListId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TaskPriority Priority { get; init; }

    public DateTime? DueDateUtc { get; init; }

    public int Position { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public IReadOnlyCollection<string> Labels { get; init; } = [];

    public IReadOnlyCollection<UserDto> AssignedUsers { get; init; } = [];

    public IReadOnlyCollection<CommentDto> Comments { get; init; } = [];
}
