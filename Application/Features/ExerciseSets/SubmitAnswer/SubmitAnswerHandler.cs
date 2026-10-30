using Application.Common.Exceptions;
using Domain;

namespace Application.Features.ExerciseSets.SubmitAnswer;

public class SubmitAnswerHandler
{
    private readonly ISubmitAnswerRepository _repository;
    public SubmitAnswerHandler(ISubmitAnswerRepository repository)
        => _repository = repository;

    public async Task HandleAsync(SubmitAnswerRequest request)
    {
        var attempt = await _repository.GetAttemptAsync(request.AttemptId);
        if (attempt == null)
            throw new NotFoundException($"Exercise set attempt with ID {request.AttemptId} does not exist.");

        //An answer cannot be added to an attempt that is already finished.
        if (attempt.CompletedAt is not null)
            throw new ValidationException(new[] { "The exercise set attempt is already completed." });

        var exercise = await _repository.GetExerciseAsync(request.ExerciseId);
        if (exercise == null)
            throw new NotFoundException($"Exercise with ID {request.ExerciseId} does not exist.");

        //The exercise must belong to the set this attempt was started for.
        if (exercise.ExerciseSetId != attempt.ExerciseSetId)
            throw new ValidationException(new[] { "The exercise does not belong to this exercise set attempt." });

        //One answer per exercise. A second submission is rejected rather than stored twice.
        if (await _repository.HasSubmissionAsync(attempt.Id, exercise.Id))
            throw new ValidationException(new[] { "This exercise has already been answered in this attempt." });

        var isCorrect = exercise.IsAnswerCorrect(request.Answer);

        var submission = new AnswerSubmission(attempt.Id, exercise.Id, request.Answer, isCorrect);

        await _repository.AddSubmissionAsync(submission);
    }
}