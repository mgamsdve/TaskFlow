namespace TaskFlow.Blazor.Models;

public sealed class UpdateCardRequest
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool ClearDescription { get; set; }

    public TaskPriority? Priority { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public bool ClearDueDate { get; set; }

    public int? Position { get; set; }

    public Guid? ListId { get; set; }
}
