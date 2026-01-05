namespace TaskFlow.Application.DTOs;

public sealed class CommentDto
{
    public Guid Id { get; init; }

    public Guid CardId { get; init; }

    public Guid AuthorId { get; init; }

    public string AuthorName { get; init; } = string.Empty;

    public string Content { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }
}
