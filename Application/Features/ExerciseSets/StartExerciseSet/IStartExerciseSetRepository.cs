using Domain;

namespace Application.Features.ExerciseSets.StartExerciseSet;

public interface IStartExerciseSetRepository
{
    Task<Student?> GetStudentAsync(int studentId);
    Task<ExerciseSet?> GetNextUnfinishedSetAsync(int topicId, int grade, IEnumerable<int> interestIds, int studentId);
    Task<ExerciseSetAttempt> CreateExerciseSetAttemptAsync(int studentId, int exerciseSetId);
}
