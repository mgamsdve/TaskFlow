using System.Net;
using System.Net.Http.Json;
using TaskFlow.API.Contracts.Projects;
using TaskFlow.Application.DTOs;
using TaskFlow.Tests.Integration.Common;

namespace TaskFlow.Tests.Integration.Endpoints;

public sealed class ProjectEndpointsTests : IClassFixture<TaskFlowApiFactory>
{
    private readonly TaskFlowApiFactory _factory;

    public ProjectEndpointsTests(TaskFlowApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateProject_ShouldCreateAndReturnProject()
    {
        using var client = _factory.CreateClient();

        var auth = await ApiAuthTestHelper.RegisterAsync(client);
        ApiAuthTestHelper.SetBearerToken(client, auth.Token);

        var request = new CreateProjectRequest
        {
            OrganizationId = TaskFlowApiFactory.SeedOrganizationId,
            Name = $"Project-{Guid.NewGuid():N}",
            Description = "Integration test project"
        };

        var createResponse = await client.PostAsJsonAsync("/projects", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(createdProject);
        Assert.Equal(request.OrganizationId, createdProject!.OrganizationId);
        Assert.Equal(request.Name, createdProject.Name);

        var getByIdResponse = await client.GetAsync($"/projects/{createdProject.Id}");
        getByIdResponse.EnsureSuccessStatusCode();

        var fetchedProject = await getByIdResponse.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(fetchedProject);
        Assert.Equal(createdProject.Id, fetchedProject!.Id);

        var getAllResponse = await client.GetAsync("/projects");
        getAllResponse.EnsureSuccessStatusCode();

        var allProjects = await getAllResponse.Content.ReadFromJsonAsync<List<ProjectDto>>();
        Assert.NotNull(allProjects);
        Assert.Contains(allProjects!, project => project.Id == createdProject.Id);
    }
}
