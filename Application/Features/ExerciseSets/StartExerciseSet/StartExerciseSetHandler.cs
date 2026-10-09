using Application.Common.Exceptions;

namespace Application.Features.ExerciseSets.StartExerciseSet;

public class StartExerciseSetHandler
{
    private readonly IStartExerciseSetRepository _repository;
    public StartExerciseSetHandler(IStartExerciseSetRepository repository)
        => _repository = repository;

    public async Task<StartExerciseSetResponse> HandleAsync(StartExerciseSetRequest request)
    {
        var student = await _repository.GetStudentAsync(request.StudentId);
        if (student == null)
        {
            throw new NotFoundException($"Student with ID {request.StudentId} does not exist.");
        }

        var interestId = student.Interests.Select(i => (int?)i.Id).FirstOrDefault();
        var set = await _repository.GetFirstMatchingSetAsync(request.TopicId, student.Grade, interestId);
        if (set == null)
            throw new NotFoundException($"Exercise set for topic ID {request.TopicId} does not exist.");

        var attempt = await _repository.CreateExerciseSetAttemptAsync(request.StudentId, set.Id);

        var exercises = set.Exercises
            .OrderBy(e => e.Position)
            .Select(e => new ExerciseItem(e.Id, e.Position, e.ExerciseText, e.Unit))
            .ToList();

        return new StartExerciseSetResponse(attempt.Id, set.Id, set.Title, exercises);
    }

}
