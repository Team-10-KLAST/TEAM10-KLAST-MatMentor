using Domain;

namespace Application.Features.ExerciseSets.StartExerciseSet;

public interface IStartExerciseSetRepository
{
    Task<Student?> GetStudentAsync(int studentId);
    Task<ExerciseSet?> GetFirstMatchingSetAsync(int topicId, int grade, int? interestId);
    Task<ExerciseSetAttempt> CreateExerciseSetAttemptAsync(int studentId, int exerciseSetId);
}
