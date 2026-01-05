using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public sealed class User
{
    private readonly global::System.Collections.Generic.List<Card> _assignedCards = [];
    private readonly global::System.Collections.Generic.List<Comment> _authoredComments = [];

    private User()
    {
    }

    public User(Guid id, string fullName, string email, UserRole role = UserRole.Member, string? passwordHash = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(id));
        }

        Id = id;
        FullName = ValidateRequired(fullName, nameof(fullName));
        Email = ValidateEmail(email);
        Role = role;
        PasswordHash = NormalizePasswordHash(passwordHash);
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<Card> AssignedCards => _assignedCards.AsReadOnly();

    public IReadOnlyCollection<Comment> AuthoredComments => _authoredComments.AsReadOnly();

    public void UpdateProfile(string fullName, string email)
    {
        FullName = ValidateRequired(fullName, nameof(fullName));
        Email = ValidateEmail(email);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = NormalizePasswordHash(passwordHash);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AssignToCard(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);
        card.AssignUser(this);
    }

    public void UnassignFromCard(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);
        card.UnassignUser(this);
    }

    internal void AddAssignedCardInternal(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        if (_assignedCards.Any(c => c.Id == card.Id))
        {
            return;
        }

        _assignedCards.Add(card);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void RemoveAssignedCardInternal(Card card)
    {
        ArgumentNullException.ThrowIfNull(card);

        var existing = _assignedCards.FirstOrDefault(c => c.Id == card.Id);
        if (existing is null)
        {
            return;
        }

        _assignedCards.Remove(existing);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AddAuthoredCommentInternal(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);

        if (_authoredComments.Any(c => c.Id == comment.Id))
        {
            return;
        }

        _authoredComments.Add(comment);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void RemoveAuthoredCommentInternal(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);

        var existing = _authoredComments.FirstOrDefault(c => c.Id == comment.Id);
        if (existing is null)
        {
            return;
        }

        _authoredComments.Remove(existing);
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

    private static string ValidateEmail(string email)
    {
        email = ValidateRequired(email, nameof(email));

        if (!email.Contains('@'))
        {
            throw new ArgumentException("Email must contain '@'.", nameof(email));
        }

        return email.ToLowerInvariant();
    }

    private static string NormalizePasswordHash(string? passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            return string.Empty;
        }

        return passwordHash.Trim();
    }
}
