using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class BoardConfiguration : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.ToTable("Boards");

        builder.HasKey(board => board.Id);

        builder.Property(board => board.ProjectId)
            .IsRequired();

        builder.Property(board => board.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(board => board.Position)
            .IsRequired();

        builder.Property(board => board.CreatedAtUtc)
            .IsRequired();

        builder.Property(board => board.UpdatedAtUtc);

        builder.HasIndex(board => new { board.ProjectId, board.Position });

        builder.HasMany(board => board.Lists)
            .WithOne(list => list.Board)
            .HasForeignKey(list => list.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Board.Lists))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
