using Application.Features.ExerciseSets.SubmitAnswer;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.ExerciseSets;

public class AnswerSubmissionRepository : ISubmitAnswerRepository
{
    private readonly AppDbContext _dbContext;
    public AnswerSubmissionRepository(AppDbContext dbContext) => _dbContext = dbContext;
    public async Task<ExerciseSetAttempt?> GetAttemptAsync(int attemptId)
        => await _dbContext.ExerciseSetAttempts
            .FirstOrDefaultAsync(a => a.Id == attemptId);
    public async Task<Exercise?> GetExerciseAsync(int exerciseId)
        => await _dbContext.Exercises
            .FirstOrDefaultAsync(e => e.Id == exerciseId);

    public async Task AddSubmissionAsync(AnswerSubmission submission)
    {
        _dbContext.AnswerSubmissions.Add(submission);
        await _dbContext.SaveChangesAsync();
    }
}
