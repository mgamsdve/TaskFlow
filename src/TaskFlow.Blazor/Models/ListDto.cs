namespace TaskFlow.Blazor.Models;

public sealed class ListDto
{
    public Guid Id { get; set; }

    public Guid BoardId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Position { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }

    public List<CardDto> Cards { get; set; } = [];
}
