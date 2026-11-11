using Application.Common.Exceptions;
using Domain;

namespace Application.Features.ExerciseSets.SubmitAnswer;

public class SubmitAnswerHandler
{
    private readonly ISubmitAnswerRepository _repository;
    public SubmitAnswerHandler(ISubmitAnswerRepository repository)
        => _repository = repository;

    // Saves the student's answer to one exercise in their own attempt.
    // The attempt must belong to the student and not be completed, the exercise must belong to the attempt's set,
    // and each exercise can only be answered once.
    public async Task HandleAsync(int studentId, int attemptId, SubmitAnswerRequest request)
    {
        ExerciseSetAttempt? attempt = await _repository.GetAttemptAsync(attemptId);
        if (attempt is null || attempt.StudentId != studentId)
            throw new NotFoundException($"Exercise set attempt with ID {attemptId} does not exist.");

        if (attempt.CompletedAt is not null)
            throw new ConflictException("The exercise set attempt is already completed.");

        Exercise? exercise = await _repository.GetExerciseAsync(request.ExerciseId);
        if (exercise is null)
            throw new NotFoundException($"Exercise with ID {request.ExerciseId} does not exist.");

        if (exercise.ExerciseSetId != attempt.ExerciseSetId)
            throw new ValidationException(["The exercise does not belong to this exercise set attempt."]);

        if (await _repository.HasSubmissionAsync(attempt.Id, exercise.Id))
            throw new ConflictException("This exercise has already been answered in this attempt.");

        bool isCorrect = exercise.IsAnswerCorrect(request.Answer);
        var submission = new AnswerSubmission(attempt.Id, exercise.Id, request.Answer, isCorrect);

        await _repository.AddSubmissionAsync(submission);
    }
}