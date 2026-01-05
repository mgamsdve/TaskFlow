using AutoMapper;
using FluentValidation;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.Commands.Projects;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Projects;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public sealed class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProjectCommand> _createProjectValidator;

    public ProjectService(
        IProjectRepository projectRepository,
        IMapper mapper,
        IValidator<CreateProjectCommand> createProjectValidator)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
        _createProjectValidator = createProjectValidator;
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _createProjectValidator.ValidateAndThrowAsync(command, cancellationToken);

        var project = new Project(Guid.NewGuid(), command.OrganizationId, command.Name, command.Description);
        await _projectRepository.AddAsync(project, cancellationToken);

        return _mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto?> GetProjectByIdAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (query.ProjectId == Guid.Empty)
        {
            throw new ArgumentException("Project id cannot be empty.", nameof(query));
        }

        var project = await _projectRepository.GetByIdAsync(query.ProjectId, cancellationToken);
        return project is null ? null : _mapper.Map<ProjectDto>(project);
    }
}
