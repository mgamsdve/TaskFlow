namespace TaskFlow.Blazor.Models;

public sealed class AddCommentRequest
{
    public Guid AuthorId { get; set; }

    public string Content { get; set; } = string.Empty;
}
