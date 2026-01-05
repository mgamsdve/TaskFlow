using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task AddAsync(Project project, CancellationToken cancellationToken = default);

    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
}
