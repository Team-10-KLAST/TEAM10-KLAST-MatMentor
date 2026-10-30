using Application.Common.Exceptions;

namespace Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public class CompleteExerciseSetAttemptHandler
{
    private readonly ICompleteExerciseSetAttemptRepository _repository;

    public CompleteExerciseSetAttemptHandler(
        ICompleteExerciseSetAttemptRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CompleteExerciseSetAttemptRequest request)
    {
        var attempt = await _repository.GetAttemptAsync(request.AttemptId);

        if (attempt is null)
            throw new NotFoundException($"Attempt {request.AttemptId} was not found.");

        if (attempt.CompletedAt is not null)
            throw new ValidationException(new[] { "Forsøget er allerede afsluttet." });

        attempt.Complete();

        await _repository.SaveChangesAsync();
    }
}