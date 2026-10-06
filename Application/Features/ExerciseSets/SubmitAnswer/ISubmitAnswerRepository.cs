using Domain;

namespace Application.Features.ExerciseSets.SubmitAnswer;

public interface ISubmitAnswerRepository
{
    Task<ExerciseSetAttempt?> GetAttemptAsync(int attemptId);
    Task<Exercise?> GetExerciseAsync(int exerciseId);
    Task AddSubmissionAsync(AnswerSubmission submission);
}
