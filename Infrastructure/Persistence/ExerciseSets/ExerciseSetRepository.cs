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
            .Include(s => s.Interest)
            .FirstOrDefaultAsync(s => s.Id == studentId);

    public async Task<ExerciseSet?> GetFirstMatchingSetAsync(int topicId, int grade, int? interestId)
        => await _db.ExerciseSets
            .Include(s => s.Exercises)
            .Where(s => s.TopicId == topicId && s.Grade == grade)
            .Where(s => interestId == null || s.InterestId == interestId)
            .OrderBy(s => s.Id)
            .FirstOrDefaultAsync();

    public async Task<ExerciseSetAttempt> CreateExerciseSetAttemptAsync(int studentId, int exerciseSetId)
    {
        var attempt = new ExerciseSetAttempt(studentId, exerciseSetId);

        _db.ExerciseSetAttempts.Add(attempt);
        await _db.SaveChangesAsync();
        
        return attempt;
    }
}
