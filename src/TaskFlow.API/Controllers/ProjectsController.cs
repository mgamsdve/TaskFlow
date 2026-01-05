using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Contracts.Projects;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.Commands.Projects;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Projects;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ProjectsController(IProjectService projectService, AppDbContext dbContext, IMapper mapper)
    {
        _projectService = projectService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var organizationExists = await _dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(organization => organization.Id == request.OrganizationId, cancellationToken);

        if (!organizationExists)
        {
            return BadRequest($"Organization '{request.OrganizationId}' was not found.");
        }

        var command = new CreateProjectCommand(request.OrganizationId, request.Name, request.Description);
        var created = await _projectService.CreateProjectAsync(command, cancellationToken);

        return CreatedAtAction(nameof(GetProjectById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProjectDto>>> GetProjects(CancellationToken cancellationToken)
    {
        var projects = await _dbContext.Projects
            .AsNoTracking()
            .AsSplitQuery()
            .Include(project => project.Boards)
                .ThenInclude(board => board.Lists)
                    .ThenInclude(list => list.Cards)
            .ToListAsync(cancellationToken);

        return Ok(_mapper.Map<List<ProjectDto>>(projects));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetProjectById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetProjectByIdAsync(new GetProjectByIdQuery(id), cancellationToken);
        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }
}
