namespace TaskFlow.Domain.Entities;

public sealed class Organization
{
    private readonly global::System.Collections.Generic.List<Project> _projects = [];

    private Organization()
    {
    }

    public Organization(Guid id, string name, string? description = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Organization id cannot be empty.", nameof(id));
        }

        Id = id;
        Name = ValidateRequired(name, nameof(name));
        Description = description?.Trim();
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

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

    public void AddProject(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);

        if (_projects.Any(p => p.Id == project.Id))
        {
            return;
        }

        project.AttachToOrganization(this);
        _projects.Add(project);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RemoveProject(Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("Project id cannot be empty.", nameof(projectId));
        }

        var existing = _projects.FirstOrDefault(p => p.Id == projectId);
        if (existing is null)
        {
            return;
        }

        _projects.Remove(existing);
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
