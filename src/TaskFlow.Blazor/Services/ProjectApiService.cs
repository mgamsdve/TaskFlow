using TaskFlow.Blazor.Models;

namespace TaskFlow.Blazor.Services;

public sealed class ProjectApiService : ApiServiceBase
{
    public ProjectApiService(HttpClient httpClient, AuthService authService)
        : base(httpClient, authService)
    {
    }

    public async Task<IReadOnlyCollection<ProjectDto>> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        var projects = await SendAsync<List<ProjectDto>>(HttpMethod.Get, "projects", cancellationToken: cancellationToken);
        return projects ?? [];
    }

    public Task<ProjectDto?> GetProjectByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return SendAsync<ProjectDto>(HttpMethod.Get, $"projects/{projectId}", cancellationToken: cancellationToken);
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default)
    {
        var project = await SendAsync<ProjectDto>(HttpMethod.Post, "projects", request, cancellationToken);
        return project ?? throw new InvalidOperationException("API response was empty while creating project.");
    }
}
