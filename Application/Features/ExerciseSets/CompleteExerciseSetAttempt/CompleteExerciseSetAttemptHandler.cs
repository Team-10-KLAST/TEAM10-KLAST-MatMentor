using Application.Common.Exceptions;
using Domain;

namespace Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public class CompleteExerciseSetAttemptHandler
{
    private readonly ICompleteExerciseSetAttemptRepository _repository;

    public CompleteExerciseSetAttemptHandler(
        ICompleteExerciseSetAttemptRepository repository)
    {
        _repository = repository;
    }

    // Completes the student's own attempt. Completing an attempt twice is a conflict.
    public async Task HandleAsync(int studentId, int attemptId)
    {
        ExerciseSetAttempt? attempt = await _repository.GetAttemptAsync(attemptId);
        if (attempt is null || attempt.StudentId != studentId)
            throw new NotFoundException($"Attempt {attemptId} was not found.");

        if (attempt.CompletedAt is not null)
            throw new ConflictException("The exercise set attempt is already completed.");

        attempt.Complete();
        await _repository.SaveChangesAsync();
    }
}