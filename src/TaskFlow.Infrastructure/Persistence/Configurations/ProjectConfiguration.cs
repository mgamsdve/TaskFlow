using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.OrganizationId)
            .IsRequired();

        builder.Property(project => project.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(1000);

        builder.Property(project => project.CreatedAtUtc)
            .IsRequired();

        builder.Property(project => project.UpdatedAtUtc);

        builder.HasIndex(project => project.OrganizationId);

        builder.HasMany(project => project.Boards)
            .WithOne(board => board.Project)
            .HasForeignKey(board => board.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Project.Boards))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
