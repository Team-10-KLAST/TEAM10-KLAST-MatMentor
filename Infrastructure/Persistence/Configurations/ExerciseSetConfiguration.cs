using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExerciseSetConfiguration : IEntityTypeConfiguration<ExerciseSet>
{
    // Configures the ExerciseSets table, its grade constraint and its relations to topic, interests and exercises.
    public void Configure(EntityTypeBuilder<ExerciseSet> builder)
    {
        builder.ToTable("ExerciseSets", t =>
            t.HasCheckConstraint("CK_ExerciseSets_Grade", "[Grade] BETWEEN 7 AND 9"));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(s => s.Topic)
            .WithMany()
            .HasForeignKey(s => s.TopicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Interest)
            .WithMany()
            .HasForeignKey(s => s.InterestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Exercises)
            .WithOne()
            .HasForeignKey(e => e.ExerciseSetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Exercises)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

}
