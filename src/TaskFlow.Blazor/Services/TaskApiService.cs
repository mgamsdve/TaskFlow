using TaskFlow.Blazor.Models;

namespace TaskFlow.Blazor.Services;

public sealed class TaskApiService : ApiServiceBase
{
    public TaskApiService(HttpClient httpClient, AuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<CardDto> CreateTaskAsync(CreateCardRequest request, CancellationToken cancellationToken = default)
    {
        var card = await SendAsync<CardDto>(HttpMethod.Post, "cards", request, cancellationToken);
        return card ?? throw new InvalidOperationException("API response was empty while creating card.");
    }

    public async Task<CardDto> UpdateTaskAsync(Guid cardId, UpdateCardRequest request, CancellationToken cancellationToken = default)
    {
        var card = await SendAsync<CardDto>(HttpMethod.Patch, $"cards/{cardId}", request, cancellationToken);
        return card ?? throw new InvalidOperationException("API response was empty while updating card.");
    }

    public Task DeleteTaskAsync(Guid cardId, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Delete, $"cards/{cardId}", cancellationToken: cancellationToken);
    }

    public Task AssignUserAsync(Guid cardId, Guid userId, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Post, $"cards/{cardId}/assign", new AssignUserRequest { UserId = userId }, cancellationToken);
    }

    public async Task<CommentDto> AddCommentAsync(Guid cardId, AddCommentRequest request, CancellationToken cancellationToken = default)
    {
        var comment = await SendAsync<CommentDto>(HttpMethod.Post, $"cards/{cardId}/comment", request, cancellationToken);
        return comment ?? throw new InvalidOperationException("API response was empty while adding comment.");
    }

    public Task<CardDto> MoveTaskAsync(Guid cardId, Guid targetListId, int targetPosition, CancellationToken cancellationToken = default)
    {
        return UpdateTaskAsync(
            cardId,
            new UpdateCardRequest
            {
                ListId = targetListId,
                Position = Math.Max(targetPosition, 0)
            },
            cancellationToken);
    }
}
