namespace TaskFlow.API.Contracts.Boards;

public sealed class CreateBoardRequest
{
    public Guid ProjectId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Position { get; init; }
}
