namespace TaskFlow.Domain.Entities;

public sealed class Label
{
    private readonly global::System.Collections.Generic.List<Card> _cards = [];

    private Label()
    {
    }

    public Label(Guid id, string name, string colorHex)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Label id cannot be empty.", nameof(id));
        }

        Id = id;
        Name = ValidateRequired(name, nameof(name));
        ColorHex = NormalizeHex(colorHex);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string ColorHex { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<Card> Cards => _cards.AsReadOnly();

    public void Rename(string name)
    {
        Name = ValidateRequired(name, nameof(name));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeColor(string colorHex)
    {
        ColorHex = NormalizeHex(colorHex);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AddCardInternal(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        if (_cards.Any(c => c.Id == card.Id))
        {
            return;
        }

        _cards.Add(card);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void RemoveCardInternal(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        var existing = _cards.FirstOrDefault(c => c.Id == card.Id);
        if (existing is null)
        {
            return;
        }

        _cards.Remove(existing);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string ValidateRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty.", paramName);
        }

        return value.Trim();
    }

    private static string NormalizeHex(string colorHex)
    {
        colorHex = ValidateRequired(colorHex, nameof(colorHex));
        if (!colorHex.StartsWith('#'))
        {
            colorHex = $"#{colorHex}";
        }

        if (colorHex.Length != 7 || !colorHex.Skip(1).All(Uri.IsHexDigit))
        {
            throw new ArgumentException("Color must be a valid 6-digit HEX value.", nameof(colorHex));
        }

        return colorHex.ToUpperInvariant();
    }
}
