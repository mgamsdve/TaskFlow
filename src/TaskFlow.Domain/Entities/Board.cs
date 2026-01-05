namespace TaskFlow.Domain.Entities;

public sealed class Board
{
    private readonly global::System.Collections.Generic.List<List> _lists = [];

    private Board()
    {
    }

    public Board(Guid id, Guid projectId, string name, int position = 0)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Board id cannot be empty.", nameof(id));
        }

        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("Project id cannot be empty.", nameof(projectId));
        }

        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative.");
        }

        Id = id;
        ProjectId = projectId;
        Name = ValidateRequired(name, nameof(name));
        Position = position;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid ProjectId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Position { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public Project? Project { get; private set; }

    public IReadOnlyCollection<List> Lists => _lists.AsReadOnly();

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

    public void AddList(List list)
    {
        ArgumentNullException.ThrowIfNull(list);

        if (_lists.Any(l => l.Id == list.Id))
        {
            return;
        }

        list.AttachToBoard(this);
        _lists.Add(list);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RemoveList(Guid listId)
    {
        if (listId == Guid.Empty)
        {
            throw new ArgumentException("List id cannot be empty.", nameof(listId));
        }

        var existing = _lists.FirstOrDefault(l => l.Id == listId);
        if (existing is null)
        {
            return;
        }

        _lists.Remove(existing);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AttachToProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        Project = project;
        ProjectId = project.Id;
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
