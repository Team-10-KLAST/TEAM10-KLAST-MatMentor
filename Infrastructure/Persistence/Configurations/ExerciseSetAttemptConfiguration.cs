using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Persistence.Configurations;

public class ExerciseSetAttemptConfiguration : IEntityTypeConfiguration<ExerciseSetAttempt>
{
    public void Configure(EntityTypeBuilder<ExerciseSetAttempt> builder)
    {
        builder.ToTable("ExerciseSetAttempts");
        builder.HasKey(a => a.Id);

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ExerciseSet>()
            .WithMany()
            .HasForeignKey(a => a.ExerciseSetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
