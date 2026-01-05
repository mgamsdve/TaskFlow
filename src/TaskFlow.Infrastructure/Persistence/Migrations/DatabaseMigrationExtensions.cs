using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Migrations;

public static class DatabaseMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            return;
        }

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
