namespace TaskFlow.API.Contracts.Lists;

public sealed class CreateListRequest
{
    public Guid BoardId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int Position { get; init; }
}
