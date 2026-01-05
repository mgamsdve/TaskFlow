using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels");

        builder.HasKey(label => label.Id);

        builder.Property(label => label.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(label => label.ColorHex)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(label => label.CreatedAtUtc)
            .IsRequired();

        builder.Property(label => label.UpdatedAtUtc);

        builder.Metadata.FindSkipNavigation(nameof(Label.Cards))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
