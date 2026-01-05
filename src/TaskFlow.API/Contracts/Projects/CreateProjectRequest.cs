namespace TaskFlow.API.Contracts.Projects;

public sealed class CreateProjectRequest
{
    public Guid OrganizationId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}
