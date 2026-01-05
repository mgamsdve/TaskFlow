namespace TaskFlow.Application.Commands.Projects;

public sealed record CreateProjectCommand(
    Guid OrganizationId,
    string Name,
    string? Description);
