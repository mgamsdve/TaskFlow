using TaskFlow.Blazor.Models;

namespace TaskFlow.Blazor.Services;

public sealed class BoardApiService : ApiServiceBase
{
    public BoardApiService(HttpClient httpClient, AuthService authService)
        : base(httpClient, authService)
    {
    }

    public Task<BoardDto?> GetBoardAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        return SendAsync<BoardDto>(HttpMethod.Get, $"boards/{boardId}", cancellationToken: cancellationToken);
    }

    public async Task<BoardDto> CreateBoardAsync(CreateBoardRequest request, CancellationToken cancellationToken = default)
    {
        var board = await SendAsync<BoardDto>(HttpMethod.Post, "boards", request, cancellationToken);
        return board ?? throw new InvalidOperationException("API response was empty while creating board.");
    }

    public async Task<ListDto> CreateListAsync(CreateListRequest request, CancellationToken cancellationToken = default)
    {
        var list = await SendAsync<ListDto>(HttpMethod.Post, "lists", request, cancellationToken);
        return list ?? throw new InvalidOperationException("API response was empty while creating list.");
    }
}
