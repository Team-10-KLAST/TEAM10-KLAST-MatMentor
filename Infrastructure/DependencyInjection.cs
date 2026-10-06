using Application.Common.Interfaces;
using Application.Features.Students.CreateStudentProfile;
using Infrastructure.Identity;
using Infrastructure.Persistence.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Features.Students.CreateStudentProfile;
using Application.Features.Topics.GetTopics;
using Application.Features.ExerciseSets.StartExerciseSet;
using Application.Features.ExerciseSets.SubmitAnswer;
using Infrastructure.Persistence.Students;
using Infrastructure.Persistence.Topics;
using Infrastructure.Persistence.ExerciseSets;

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
        .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<ICreateStudentProfileRepository, StudentRepository>();
        services.AddScoped<IGetTopicsRepository, TopicRepository>();
        services.AddScoped<IStartExerciseSetRepository, ExerciseSetRepository>();
        services.AddScoped<ISubmitAnswerRepository, AnswerSubmissionRepository>();

        return services;
    }
}