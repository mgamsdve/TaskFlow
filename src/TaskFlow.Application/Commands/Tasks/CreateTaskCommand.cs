using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Commands.Tasks;

public sealed record CreateTaskCommand(
    Guid ListId,
    string Title,
    string? Description,
    TaskPriority Priority,
    int Position,
    DateTime? DueDateUtc);
