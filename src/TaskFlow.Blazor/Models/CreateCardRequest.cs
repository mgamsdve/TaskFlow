using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Blazor.Models;

public sealed class CreateCardRequest
{
    [Required]
    public Guid ListId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Range(0, int.MaxValue)]
    public int Position { get; set; }

    public DateTime? DueDateUtc { get; set; }
}
