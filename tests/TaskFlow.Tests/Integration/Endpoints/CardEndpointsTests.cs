using System.Net.Http.Json;
using TaskFlow.API.Contracts.Cards;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Enums;
using TaskFlow.Tests.Integration.Common;

namespace TaskFlow.Tests.Integration.Endpoints;

public sealed class CardEndpointsTests : IClassFixture<TaskFlowApiFactory>
{
    private readonly TaskFlowApiFactory _factory;

    public CardEndpointsTests(TaskFlowApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTask_And_GetBoardTasks_ShouldIncludeCreatedTask()
    {
        using var client = _factory.CreateClient();

        var auth = await ApiAuthTestHelper.RegisterAsync(client);
        ApiAuthTestHelper.SetBearerToken(client, auth.Token);

        var createRequest = new CreateCardRequest
        {
            ListId = TaskFlowApiFactory.SeedListId,
            Title = $"Task-{Guid.NewGuid():N}",
            Description = "Task created by integration test",
            Priority = TaskPriority.High,
            Position = 0
        };

        var createResponse = await client.PostAsJsonAsync("/cards", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var createdCard = await createResponse.Content.ReadFromJsonAsync<CardDto>();
        Assert.NotNull(createdCard);

        var boardResponse = await client.GetAsync($"/boards/{TaskFlowApiFactory.SeedBoardId}");
        boardResponse.EnsureSuccessStatusCode();

        var board = await boardResponse.Content.ReadFromJsonAsync<BoardDto>();
        Assert.NotNull(board);

        var cardIds = board!.Lists.SelectMany(list => list.Cards).Select(card => card.Id);
        Assert.Contains(createdCard!.Id, cardIds);
    }

    [Fact]
    public async Task AssignUserToTask_ShouldAppearInBoardTaskAssignees()
    {
        using var client = _factory.CreateClient();

        var ownerAuth = await ApiAuthTestHelper.RegisterAsync(client, UserRole.Owner, $"owner-{Guid.NewGuid():N}@taskflow.test");
        var memberAuth = await ApiAuthTestHelper.RegisterAsync(client, UserRole.Member, $"member-{Guid.NewGuid():N}@taskflow.test");

        ApiAuthTestHelper.SetBearerToken(client, ownerAuth.Token);

        var createTaskResponse = await client.PostAsJsonAsync(
            "/cards",
            new CreateCardRequest
            {
                ListId = TaskFlowApiFactory.SeedListId,
                Title = $"Assignable-{Guid.NewGuid():N}",
                Description = "Task to assign",
                Priority = TaskPriority.Medium,
                Position = 1
            });

        createTaskResponse.EnsureSuccessStatusCode();

        var createdCard = await createTaskResponse.Content.ReadFromJsonAsync<CardDto>();
        Assert.NotNull(createdCard);

        var assignResponse = await client.PostAsJsonAsync(
            $"/cards/{createdCard!.Id}/assign",
            new AssignUserRequest { UserId = memberAuth.User.Id });

        assignResponse.EnsureSuccessStatusCode();

        var boardResponse = await client.GetAsync($"/boards/{TaskFlowApiFactory.SeedBoardId}");
        boardResponse.EnsureSuccessStatusCode();

        var board = await boardResponse.Content.ReadFromJsonAsync<BoardDto>();
        Assert.NotNull(board);

        var updatedCard = board!.Lists
            .SelectMany(list => list.Cards)
            .First(card => card.Id == createdCard.Id);

        Assert.Contains(updatedCard.AssignedUsers, assigned => assigned.Id == memberAuth.User.Id);
    }
}
