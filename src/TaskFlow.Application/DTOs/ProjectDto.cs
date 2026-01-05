namespace TaskFlow.Application.DTOs;

public sealed class ProjectDto
{
    public Guid Id { get; init; }

    public Guid OrganizationId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public DateTime CreatedAtUtc { get; init; }

    public DateTime? UpdatedAtUtc { get; init; }

    public IReadOnlyCollection<BoardDto> Boards { get; init; } = [];
}
