using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.CardId)
            .IsRequired();

        builder.Property(comment => comment.AuthorId)
            .IsRequired();

        builder.Property(comment => comment.Content)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(comment => comment.CreatedAtUtc)
            .IsRequired();

        builder.Property(comment => comment.UpdatedAtUtc);

        builder.HasOne(comment => comment.Card)
            .WithMany(card => card.Comments)
            .HasForeignKey(comment => comment.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(comment => comment.Author)
            .WithMany(user => user.AuthoredComments)
            .HasForeignKey(comment => comment.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
