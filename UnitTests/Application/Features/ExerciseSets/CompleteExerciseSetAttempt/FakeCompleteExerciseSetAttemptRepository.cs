using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Domain;

namespace UnitTests.Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

// Supplies attempts to the handler without a database. Saving is not checked here,
// because whether the attempt is stored is covered by the integration tests
internal sealed class FakeCompleteExerciseSetAttemptRepository : ICompleteExerciseSetAttemptRepository
{
    private readonly Dictionary<int, ExerciseSetAttempt> _attempts = new();

    public void AddAttempt(int attemptId, ExerciseSetAttempt attempt)
        => _attempts[attemptId] = attempt;

    public Task<ExerciseSetAttempt?> GetAttemptAsync(int attemptId)
        => Task.FromResult(_attempts.GetValueOrDefault(attemptId));

    public Task SaveChangesAsync()
        => Task.CompletedTask;
}