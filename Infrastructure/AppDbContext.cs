using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseSetAttempt> ExerciseSetAttempts => Set<ExerciseSetAttempt>();
    public DbSet<AnswerSubmission> AnswerSubmissions => Set<AnswerSubmission>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedData.Apply(modelBuilder);
    }
}
