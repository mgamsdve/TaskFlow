namespace TaskFlow.Application.Commands.Tasks;

public sealed record AssignUserToTaskCommand(
    Guid CardId,
    Guid UserId);
