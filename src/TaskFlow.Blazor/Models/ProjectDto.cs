namespace TaskFlow.Blazor.Models;

public sealed class ProjectDto
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public List<BoardDto> Boards { get; set; } = [];
}
