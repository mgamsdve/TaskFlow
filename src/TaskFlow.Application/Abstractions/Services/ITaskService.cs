using TaskFlow.Application.Commands.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Tasks;

namespace TaskFlow.Application.Abstractions.Services;

public interface ITaskService
{
    Task<CardDto> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);

    Task AssignUserToTaskAsync(AssignUserToTaskCommand command, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CardDto>> GetTasksByListAsync(GetTasksByListQuery query, CancellationToken cancellationToken = default);
}
