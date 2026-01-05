using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        if (projectId == Guid.Empty)
        {
            return null;
        }

        return await _dbContext.Projects
            .AsSplitQuery()
            .Include(project => project.Boards)
                .ThenInclude(board => board.Lists)
                    .ThenInclude(list => list.Cards)
            .FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        await _dbContext.Projects.AddAsync(project, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(project);

        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
