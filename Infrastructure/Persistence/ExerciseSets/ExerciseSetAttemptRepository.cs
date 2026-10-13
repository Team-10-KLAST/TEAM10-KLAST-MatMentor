using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.ExerciseSets;

public class ExerciseSetAttemptRepository : ICompleteExerciseSetAttemptRepository
{
    private readonly AppDbContext _context;

    public ExerciseSetAttemptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ExerciseSetAttempt?> GetAttemptAsync(int attemptId)
    {
        return await _context.ExerciseSetAttempts
            .FirstOrDefaultAsync(a => a.Id == attemptId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}