using Application.Features.ExerciseSets.StartExerciseSet;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.ExerciseSets;

public class ExerciseSetRepository : IStartExerciseSetRepository
{
    private readonly AppDbContext _db;
    public ExerciseSetRepository(AppDbContext db) => _db = db;
    public async Task<Student?> GetStudentAsync(int studentId)
                => await _db.Students
            .Include(s => s.Interests)
            .FirstOrDefaultAsync(s => s.Id == studentId);

    // Returns the next unfinished exercise set without an interest (standard set) for the topic and grade
    public async Task<ExerciseSet?> GetNextUnfinishedStandardSetAsync(int topicId, int grade, int studentId)
    {
        return await UnfinishedSets(topicId, grade, studentId)
            .Where(s => s.InterestId == null)
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync();
    }

    // Returns the next unfinished exercise set matching one of the student's interests for the topic and grade
    public async Task<ExerciseSet?> GetNextUnfinishedInterestSetAsync(
        int topicId, int grade, IReadOnlyList<int> interestIds, int studentId)
    {
        return await UnfinishedSets(topicId, grade, studentId)
            .Where(s => s.InterestId != null && interestIds.Contains(s.InterestId.Value))
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync();
    }

    // Builds the shared query: sets for the topic and grade that the student has not completed
    private IQueryable<ExerciseSet> UnfinishedSets(int topicId, int grade, int studentId)
    {
        return _db.ExerciseSets
            .Include(s => s.Exercises)
            .Where(s => s.TopicId == topicId
                     && s.Grade == grade
                     && !_db.ExerciseSetAttempts
                            .Any(a => a.StudentId == studentId
                                   && a.ExerciseSetId == s.Id
                                   && a.CompletedAt != null));
    }

    public async Task<ExerciseSetAttempt> CreateExerciseSetAttemptAsync(int studentId, int exerciseSetId)
    {
        var attempt = new ExerciseSetAttempt(studentId, exerciseSetId);

        _db.ExerciseSetAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        return attempt;
    }
}
