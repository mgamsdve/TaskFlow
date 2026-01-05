namespace TaskFlow.Application.DTOs;

public sealed class ListDto
{
    public Guid Id { get; init; }

    public Guid BoardId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Position { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public IReadOnlyCollection<CardDto> Cards { get; init; } = [];
}
