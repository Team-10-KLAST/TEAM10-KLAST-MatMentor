using Application.Common.Interfaces;
using Application.Features.ChatTutor.AskMathQuestion;
using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Application.Features.ExerciseSets.StartExerciseSet;
using Application.Features.ExerciseSets.SubmitAnswer;
using Application.Features.Students.CreateStudentProfile;
using Application.Features.Topics.GetTopics;
using Infrastructure.Identity;
using Infrastructure.Persistence.ExerciseSets;
using Infrastructure.Persistence.Students;
using Infrastructure.Persistence.Topics;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructure;

using Google.GenAI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.User.RequireUniqueEmail = false;

            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789-._";

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddSignInManager()
        .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<ICreateStudentProfileRepository, StudentRepository>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IGetTopicsRepository, TopicRepository>();
        services.AddScoped<IStartExerciseSetRepository, ExerciseSetRepository>();
        services.AddScoped<ISubmitAnswerRepository, AnswerSubmissionRepository>();
        services.AddScoped<ICompleteExerciseSetAttemptRepository, ExerciseSetAttemptRepository>();
        services.AddScoped<ITokenService, TokenService>();

        // Register AI Math Service
        services.AddScoped<IAiMathService, GeminiAiMathService>();

        // Pull API Key from user secrets or environment variables safely
        string apiKey = configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API Key is not configured.");
        }

        // Register Google GenAI Client as a Singleton
        services.AddSingleton(sp => new Client(apiKey: apiKey));

        return services;
    }
}