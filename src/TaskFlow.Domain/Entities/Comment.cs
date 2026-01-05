namespace TaskFlow.Domain.Entities;

public sealed class Comment
{
    private Comment()
    {
    }

    public Comment(Guid id, Guid cardId, User author, string content)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Comment id cannot be empty.", nameof(id));
        }

        if (cardId == Guid.Empty)
        {
            throw new ArgumentException("Card id cannot be empty.", nameof(cardId));
        }

        ArgumentNullException.ThrowIfNull(author);

        Id = id;
        CardId = cardId;
        AuthorId = author.Id;
        Author = author;
        Content = ValidateRequired(content, nameof(content));
        CreatedAtUtc = DateTime.UtcNow;

        author.AddAuthoredCommentInternal(this);
    }

    public Guid Id { get; private set; }

    public Guid CardId { get; private set; }

    public Guid AuthorId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public Card? Card { get; private set; }

    public User? Author { get; private set; }

    public void Edit(string content)
    {
        Content = ValidateRequired(content, nameof(content));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AttachToCard(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        Card = card;
        CardId = card.Id;
    }

    internal void DetachFromCard()
    {
        Card = null;
    }

    internal void AttachAuthor(User author)
    {
        ArgumentNullException.ThrowIfNull(author);

        if (Author?.Id == author.Id)
        {
            return;
        }

        Author?.RemoveAuthoredCommentInternal(this);
        Author = author;
        AuthorId = author.Id;
        author.AddAuthoredCommentInternal(this);
    }

    private static string ValidateRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty.", paramName);
        }

        return value.Trim();
    }
}
