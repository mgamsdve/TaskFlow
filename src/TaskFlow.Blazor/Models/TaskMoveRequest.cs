namespace TaskFlow.Blazor.Models;

public sealed class TaskMoveRequest
{
    public Guid CardId { get; set; }

    public Guid ToListId { get; set; }

    public int Position { get; set; }
}
