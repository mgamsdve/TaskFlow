namespace TaskFlow.Domain.Entities;

public sealed class List
{
    private readonly global::System.Collections.Generic.List<Card> _cards = [];

    private List()
    {
    }

    public List(Guid id, Guid boardId, string name, int position = 0)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("List id cannot be empty.", nameof(id));
        }

        if (boardId == Guid.Empty)
        {
            throw new ArgumentException("Board id cannot be empty.", nameof(boardId));
        }

        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
        }

        Id = id;
        BoardId = boardId;
        Name = ValidateRequired(name, nameof(name));
        Position = position;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid BoardId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Position { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public Board? Board { get; private set; }

    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    public void Rename(string name)
    {
        Name = ValidateRequired(name, nameof(name));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Reposition(int position)
    {
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
        }

        Position = position;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AddCard(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        if (_cards.Any(c => c.Id == card.Id))
        {
            return;
        }

        card.AttachToList(this);
        _cards.Add(card);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RemoveCard(Guid cardId)
    {
        if (cardId == Guid.Empty)
        {
            throw new ArgumentException("Card id cannot be empty.", nameof(cardId));
        }

        var existing = _cards.FirstOrDefault(c => c.Id == cardId);
        if (existing is null)
        {
            return;
        }

        _cards.Remove(existing);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AttachToBoard(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        Board = board;
        BoardId = board.Id;
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
