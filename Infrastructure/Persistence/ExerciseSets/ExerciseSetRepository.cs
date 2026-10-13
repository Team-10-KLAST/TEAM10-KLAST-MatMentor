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

    public async Task<ExerciseSet?> GetNextUnfinishedSetAsync(
       int topicId, int grade, IEnumerable<int> interestIds, int studentId)
    {
        return await _db.ExerciseSets
            .Include(s => s.Exercises)
            .Where(s => s.TopicId == topicId
                     && s.Grade == grade
                     && interestIds.Contains(s.InterestId)
                     && !_db.ExerciseSetAttempts
                            .Any(a => a.StudentId == studentId
                                   && a.ExerciseSetId == s.Id
                                   && a.CompletedAt != null))
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync();
    }
    public async Task<ExerciseSetAttempt> CreateExerciseSetAttemptAsync(int studentId, int exerciseSetId)
    {
        var attempt = new ExerciseSetAttempt(studentId, exerciseSetId);

        _db.ExerciseSetAttempts.Add(attempt);
        await _db.SaveChangesAsync();

        return attempt;
    }
}
