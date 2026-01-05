namespace TaskFlow.Domain.Entities;

public sealed class Project
{
    private readonly global::System.Collections.Generic.List<Board> _boards = [];

    private Project()
    {
    }

    public Project(Guid id, Guid organizationId, string name, string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Project id cannot be empty.", nameof(id));
        }

        if (organizationId == Guid.Empty)
        {
            throw new ArgumentException("Organization id cannot be empty.", nameof(organizationId));
        }

        Id = id;
        OrganizationId = organizationId;
        Name = ValidateRequired(name, nameof(name));
        Description = description?.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public Guid OrganizationId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public Organization? Organization { get; private set; }

    public IReadOnlyCollection<Board> Boards => _boards.AsReadOnly();

    public void Rename(string name)
    {
        Name = ValidateRequired(name, nameof(name));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AddBoard(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        if (_boards.Any(b => b.Id == board.Id))
        {
            return;
        }

        board.AttachToProject(this);
        _boards.Add(board);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RemoveBoard(Guid boardId)
    {
        if (boardId == Guid.Empty)
        {
            throw new ArgumentException("Board id cannot be empty.", nameof(boardId));
        }

        var existing = _boards.FirstOrDefault(b => b.Id == boardId);
        if (existing is null)
        {
            return;
        }

        _boards.Remove(existing);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    internal void AttachToOrganization(Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);

        Organization = organization;
        OrganizationId = organization.Id;
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
