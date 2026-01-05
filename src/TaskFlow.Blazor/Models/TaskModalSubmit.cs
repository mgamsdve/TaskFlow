namespace TaskFlow.Blazor.Models;

public sealed class TaskModalSubmit
{
    public bool IsEditMode { get; set; }

    public Guid? CardId { get; set; }

    public CreateCardRequest? CreateRequest { get; set; }

    public UpdateCardRequest? UpdateRequest { get; set; }
}
