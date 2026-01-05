namespace TaskFlow.Blazor.Models;

public sealed class CardDto
{
    public Guid Id { get; set; }

    public Guid ListId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskPriority Priority { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public int Position { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public List<string> Labels { get; set; } = [];

    public List<UserDto> AssignedUsers { get; set; } = [];

    public List<CommentDto> Comments { get; set; } = [];
}
