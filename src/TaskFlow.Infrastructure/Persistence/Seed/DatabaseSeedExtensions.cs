using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.Infrastructure.Persistence.Seed;

public static class DatabaseSeedExtensions
{
    public static readonly Guid DefaultOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static async Task SeedDefaultsAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var organizationExists = await dbContext.Organizations
            .AsNoTracking()
            .AnyAsync(organization => organization.Id == DefaultOrganizationId, cancellationToken);

        if (organizationExists)
        {
            return;
        }

        var defaultOrganization = new Organization(
            DefaultOrganizationId,
            "TaskFlow Default Organization",
            "Auto-created default organization for local development.");

        dbContext.Organizations.Add(defaultOrganization);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
