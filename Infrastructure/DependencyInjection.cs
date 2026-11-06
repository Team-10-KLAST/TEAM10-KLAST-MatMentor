using Application.Features.ChatTutor.AskTutor;
using Infrastructure.AI;
using Application.Common.Interfaces;
using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Application.Features.ExerciseSets.StartExerciseSet;
using Application.Features.ExerciseSets.SubmitAnswer;
using Application.Features.Students.CreateStudentProfile;
using Application.Features.Topics.GetTopics;
using Infrastructure.Identity;
using Infrastructure.Persistence.ExerciseSets;
using Infrastructure.Persistence.Students;
using Infrastructure.Persistence.Topics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Infrastructure;

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
        services.AddHttpClient<ITutorModelClient, OllamaTutorModelClient>((serviceProvider, client) =>
        {
            IConfiguration settings = serviceProvider.GetRequiredService<IConfiguration>();
            string url = settings["Ollama:BaseUrl"]
                ?? "http://127.0.0.1:11434/";
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        return services;
    }
}