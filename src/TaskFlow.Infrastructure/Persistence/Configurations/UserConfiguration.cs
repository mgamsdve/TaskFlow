using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder.Property(user => user.UpdatedAtUtc);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Metadata.FindNavigation(nameof(User.AuthoredComments))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindSkipNavigation(nameof(User.AssignedCards))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
