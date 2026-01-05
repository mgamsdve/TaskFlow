using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Blazor.Models;

public sealed class CreateListRequest
{
    [Required]
    public Guid BoardId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Position { get; set; }
}
