using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;
using TaskListEntity = TaskFlow.Domain.Entities.List;

namespace TaskFlow.Tests.Integration.Common;

public sealed class TaskFlowApiFactory : WebApplicationFactory<Program>
{
    public static readonly Guid SeedOrganizationId = Guid.Parse("f8a3a363-7d62-4d2b-8a06-0f77218b7f00");
    public static readonly Guid SeedProjectId = Guid.Parse("6d9a68af-5f59-45f9-b8cc-72d81f7a3401");
    public static readonly Guid SeedBoardId = Guid.Parse("1a2402ec-58cb-4d2c-9b95-d3b6b3ed8669");
    public static readonly Guid SeedListId = Guid.Parse("03ca8ebf-a590-4aa4-8913-e8f4a31f4d09");

    private readonly string _databaseName = $"taskflow-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            SeedData(dbContext);
        });
    }

    private static void SeedData(AppDbContext dbContext)
    {
        if (dbContext.Organizations.Any())
        {
            return;
        }

        var organization = new Organization(SeedOrganizationId, "TaskFlow Org", "Integration test organization");
        var project = new Project(SeedProjectId, SeedOrganizationId, "TaskFlow Seed Project", "Seed project");
        var board = new Board(SeedBoardId, SeedProjectId, "TaskFlow Seed Board", 0);
        var list = new TaskListEntity(SeedListId, SeedBoardId, "To Do", 0);

        dbContext.Organizations.Add(organization);
        dbContext.Projects.Add(project);
        dbContext.Boards.Add(board);
        dbContext.Lists.Add(list);

        dbContext.SaveChanges();
    }
}
