using TaskFlow.Application.Commands.Projects;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Projects;

namespace TaskFlow.Application.Abstractions.Services;

public interface IProjectService
{
    Task<ProjectDto> CreateProjectAsync(CreateProjectCommand command, CancellationToken cancellationToken = default);

    Task<ProjectDto?> GetProjectByIdAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default);
}
