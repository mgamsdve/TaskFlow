namespace TaskFlow.Application.DTOs;

public sealed class BoardDto
{
    public Guid Id { get; init; }

    public Guid ProjectId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Position { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public IReadOnlyCollection<ListDto> Lists { get; init; } = [];
}
