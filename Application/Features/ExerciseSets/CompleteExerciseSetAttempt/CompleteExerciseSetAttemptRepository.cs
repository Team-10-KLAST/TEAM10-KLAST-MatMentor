using Domain;

namespace Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public interface ICompleteExerciseSetAttemptRepository
{
    Task<ExerciseSetAttempt?> GetAttemptAsync(int attemptId);
    Task SaveChangesAsync();
}