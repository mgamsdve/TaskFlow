using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Infrastructure.Persistence.Context;
using TaskFlow.Infrastructure.Persistence.Repositories;

namespace TaskFlow.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    private const string DefaultConnectionString = "Host=localhost;Database=taskflow;Username=postgres;Password=postgres";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = DefaultConnectionString;
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ICardRepository, CardRepository>();

        return services;
    }
}
