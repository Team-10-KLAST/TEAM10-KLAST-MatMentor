using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AnswerSubmissionConfiguration : IEntityTypeConfiguration<AnswerSubmission>
{
    public void Configure(EntityTypeBuilder<AnswerSubmission> builder)
    {
        builder.ToTable("AnswerSubmissions");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.SubmittedAnswer)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne<ExerciseSetAttempt>()
             .WithMany()
             .HasForeignKey(a => a.ExerciseSetAttemptId)
             .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(a => a.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new { a.ExerciseSetAttemptId, a.ExerciseId }).IsUnique();
    }
}
