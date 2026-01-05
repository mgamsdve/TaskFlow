using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskListEntity = TaskFlow.Domain.Entities.List;

namespace TaskFlow.Infrastructure.Persistence.Context;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Board> Boards => Set<Board>();

    public DbSet<TaskListEntity> Lists => Set<TaskListEntity>();

    public DbSet<Card> Cards => Set<Card>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Label> Labels => Set<Label>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
