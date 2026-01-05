namespace TaskFlow.Blazor.Models;

public sealed class BoardDto
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Position { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public List<ListDto> Lists { get; set; } = [];
}
