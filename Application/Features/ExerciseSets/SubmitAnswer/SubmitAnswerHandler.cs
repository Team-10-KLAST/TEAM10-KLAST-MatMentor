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
        
        var exercise = await _repository.GetExerciseAsync(request.ExerciseId);
        if (exercise == null)
            throw new NotFoundException($"Exercise with ID {request.ExerciseId} does not exist.");
        
        var isCorrect = exercise.IsAnswerCorrect(request.Answer);
        
        var submission = new AnswerSubmission(attempt.Id, exercise.Id, request.Answer, isCorrect);
        
        await _repository.AddSubmissionAsync(submission);
    }
}
