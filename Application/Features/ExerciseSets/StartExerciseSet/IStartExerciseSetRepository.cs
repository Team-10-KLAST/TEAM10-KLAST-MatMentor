using Domain;

namespace Application.Features.ExerciseSets.StartExerciseSet;

public interface IStartExerciseSetRepository
{
    Task<Student?> GetStudentAsync(int studentId);
    Task<ExerciseSet?> GetNextUnfinishedStandardSetAsync(int topicId, int grade, int studentId);
    Task<ExerciseSet?> GetNextUnfinishedInterestSetAsync(int topicId, int grade, IReadOnlyList<int> interestIds, int studentId);
    Task<ExerciseSetAttempt> CreateExerciseSetAttemptAsync(int studentId, int exerciseSetId);
}
