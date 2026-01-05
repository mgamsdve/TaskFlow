using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("Cards");

        builder.HasKey(card => card.Id);

        builder.Property(card => card.ListId)
            .IsRequired();

        builder.Property(card => card.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(card => card.Description)
            .HasMaxLength(2000);

        builder.Property(card => card.Priority)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(card => card.Position)
            .IsRequired();

        builder.Property(card => card.CreatedAtUtc)
            .IsRequired();

        builder.Property(card => card.UpdatedAtUtc);

        builder.Property(card => card.DueDateUtc);

        builder.HasIndex(card => new { card.ListId, card.Position });

        builder.HasMany(card => card.Comments)
            .WithOne(comment => comment.Card)
            .HasForeignKey(comment => comment.CardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(card => card.Labels)
            .WithMany(label => label.Cards)
            .UsingEntity<Dictionary<string, object>>(
                "CardLabels",
                right => right
                    .HasOne<Label>()
                    .WithMany()
                    .HasForeignKey("LabelId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<Card>()
                    .WithMany()
                    .HasForeignKey("CardId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("CardLabels");
                    join.HasKey("CardId", "LabelId");
                });

        builder.HasMany(card => card.AssignedUsers)
            .WithMany(user => user.AssignedCards)
            .UsingEntity<Dictionary<string, object>>(
                "CardAssignments",
                right => right
                    .HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<Card>()
                    .WithMany()
                    .HasForeignKey("CardId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("CardAssignments");
                    join.HasKey("CardId", "UserId");
                });

        builder.Metadata.FindNavigation(nameof(Card.Comments))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindSkipNavigation(nameof(Card.Labels))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata.FindSkipNavigation(nameof(Card.AssignedUsers))
            ?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
