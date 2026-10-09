using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExerciseText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(e => e.CorrectAnswer)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Unit)
            .HasMaxLength(50);

        builder.HasOne<Topic>()
            .WithMany()
            .HasForeignKey(e => e.TopicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.ExerciseSetId, e.Position }).IsUnique();

    }
}
