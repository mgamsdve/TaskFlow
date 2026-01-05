namespace TaskFlow.Blazor.Models;

public sealed class CommentDto
{
    public Guid Id { get; set; }

    public Guid CardId { get; set; }

    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
