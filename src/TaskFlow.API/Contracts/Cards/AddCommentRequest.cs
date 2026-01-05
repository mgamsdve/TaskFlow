namespace TaskFlow.API.Contracts.Cards;

public sealed class AddCommentRequest
{
    public Guid AuthorId { get; init; }

    public string Content { get; init; } = string.Empty;
}
