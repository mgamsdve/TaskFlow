using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Blazor.Models;

public sealed class CreateProjectRequest
{
    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }
}
