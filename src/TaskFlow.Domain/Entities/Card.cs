using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public sealed class Card
{
    private readonly global::System.Collections.Generic.List<Comment> _comments = [];
    private readonly global::System.Collections.Generic.List<Label> _labels = [];
    private readonly global::System.Collections.Generic.List<User> _assignedUsers = [];

    private Card()
    {
    }

    public Card(Guid id, Guid listId, string title, string? description = null, TaskPriority priority = TaskPriority.Medium, int position = 0)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Card id cannot be empty.", nameof(id));
        }

        if (listId == Guid.Empty)
        {
            throw new ArgumentException("List id cannot be empty.", nameof(listId));
        }

        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
        }

        Id = id;
        ListId = listId;
        Title = ValidateRequired(title, nameof(title));
        Description = description?.Trim();
        Priority = priority;
        Position = position;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ListId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public TaskPriority Priority { get; private set; }

    public DateTime? DueDateUtc { get; private set; }

    public int Position { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public List? List { get; private set; }

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public IReadOnlyCollection<Label> Labels => _labels.AsReadOnly();

    public IReadOnlyCollection<User> AssignedUsers => _assignedUsers.AsReadOnly();

    public void Rename(string title)
    {
        Title = ValidateRequired(title, nameof(title));
        Touch();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        Touch();
    }

    public void ChangePriority(TaskPriority priority)
    {
        Priority = priority;
        Touch();
    }

    public void ScheduleDueDate(DateTime? dueDateUtc)
    {
        if (dueDateUtc.HasValue && dueDateUtc.Value.Kind == DateTimeKind.Unspecified)
        {
            throw new ArgumentException("Due date must include timezone information.", nameof(dueDateUtc));
        }

        DueDateUtc = dueDateUtc?.ToUniversalTime();
        Touch();
    }

    public void Reposition(int position)
    {
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
        }

        Position = position;
        Touch();
    }

    public void MoveToList(List list, int position)
    {
        ArgumentNullException.ThrowIfNull(list);
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
        }

        AttachToList(list);
        Position = position;
        Touch();
    }

    public void AddComment(Comment comment)
    {
        ArgumentNullException.ThrowIfNull(comment);

        if (_comments.Any(c => c.Id == comment.Id))
        {
            return;
        }

        comment.AttachToCard(this);
        _comments.Add(comment);
        Touch();
    }

    public void RemoveComment(Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            throw new ArgumentException("Comment id cannot be empty.", nameof(commentId));
        }

        var existing = _comments.FirstOrDefault(c => c.Id == commentId);
        if (existing is null)
        {
            return;
        }

        existing.DetachFromCard();
        _comments.Remove(existing);
        Touch();
    }

    public void AddLabel(Label label)
    {
        ArgumentNullException.ThrowIfNull(label);

        if (_labels.Any(l => l.Id == label.Id))
        {
            return;
        }

        _labels.Add(label);
        label.AddCardInternal(this);
        Touch();
    }

    public void RemoveLabel(Label label)
    {
        ArgumentNullException.ThrowIfNull(label);

        var existing = _labels.FirstOrDefault(l => l.Id == label.Id);
        if (existing is null)
        {
            return;
        }

        _labels.Remove(existing);
        label.RemoveCardInternal(this);
        Touch();
    }

    public void AssignUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (_assignedUsers.Any(u => u.Id == user.Id))
        {
            return;
        }

        _assignedUsers.Add(user);
        user.AddAssignedCardInternal(this);
        Touch();
    }

    public void UnassignUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var existing = _assignedUsers.FirstOrDefault(u => u.Id == user.Id);
        if (existing is null)
        {
            return;
        }

        _assignedUsers.Remove(existing);
        user.RemoveAssignedCardInternal(this);
        Touch();
    }

    internal void AttachToList(List list)
    {
        ArgumentNullException.ThrowIfNull(list);

        List = list;
        ListId = list.Id;
    }

    private void Touch()
    {
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
}
