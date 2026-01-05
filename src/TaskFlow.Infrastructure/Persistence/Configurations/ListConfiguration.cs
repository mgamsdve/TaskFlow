using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskListEntity = TaskFlow.Domain.Entities.List;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class ListConfiguration : IEntityTypeConfiguration<TaskListEntity>
{
    public void Configure(EntityTypeBuilder<TaskListEntity> builder)
    {
        builder.ToTable("Lists");

        builder.HasKey(list => list.Id);

        builder.Property(list => list.BoardId)
            .IsRequired();

        builder.Property(list => list.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(list => list.Position)
            .IsRequired();

        builder.Property(list => list.CreatedAtUtc)
            .IsRequired();

        builder.Property(list => list.UpdatedAtUtc);

        builder.HasIndex(list => new { list.BoardId, list.Position });

        builder.HasMany(list => list.Cards)
            .WithOne(card => card.List)
            .HasForeignKey(card => card.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(TaskListEntity.Cards))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
