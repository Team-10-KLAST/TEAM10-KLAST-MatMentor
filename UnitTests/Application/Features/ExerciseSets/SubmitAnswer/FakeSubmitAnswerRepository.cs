using Application.Features.ExerciseSets.SubmitAnswer;
using Domain;

namespace UnitTests.Application.Features.ExerciseSets.SubmitAnswer;

// An in-memory replacement for the database, so the handler can be tested without SQL Server
internal sealed class FakeSubmitAnswerRepository : ISubmitAnswerRepository
{
    private readonly Dictionary<int, ExerciseSetAttempt> _attempts = new();
    private readonly Dictionary<int, Exercise> _exercises = new();

    public List<AnswerSubmission> Submissions { get; } = new();

    public void AddAttempt(int attemptId, ExerciseSetAttempt attempt)
        => _attempts[attemptId] = attempt;

    public void AddExercise(int exerciseId, Exercise exercise)
        => _exercises[exerciseId] = exercise;

    public Task<ExerciseSetAttempt?> GetAttemptAsync(int attemptId)
        => Task.FromResult(_attempts.GetValueOrDefault(attemptId));

    public Task<Exercise?> GetExerciseAsync(int exerciseId)
        => Task.FromResult(_exercises.GetValueOrDefault(exerciseId));

    public Task<bool> HasSubmissionAsync(int attemptId, int exerciseId)
        => Task.FromResult(Submissions.Any(s => s.ExerciseSetAttemptId == attemptId && s.ExerciseId == exerciseId));

    public Task AddSubmissionAsync(AnswerSubmission submission)
    {
        Submissions.Add(submission);
        return Task.CompletedTask;
    }
}