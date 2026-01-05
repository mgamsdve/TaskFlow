using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(organization => organization.Id);

        builder.Property(organization => organization.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(organization => organization.Description)
            .HasMaxLength(1000);

        builder.Property(organization => organization.CreatedAtUtc)
            .IsRequired();

        builder.Property(organization => organization.UpdatedAtUtc);

        builder.HasMany(organization => organization.Projects)
            .WithOne(project => project.Organization)
            .HasForeignKey(project => project.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Organization.Projects))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
