using Application.Features.ExerciseSets.StartExerciseSet;
using Application.Features.ExerciseSets.SubmitAnswer;
using Application.Features.Students.CreateStudentProfile;
using Application.Features.Topics.GetTopics;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateStudentProfileHandler>();
        services.AddScoped<CreateStudentProfileValidator>();   // ← new line
        services.AddScoped<GetTopicsHandler>();
        services.AddScoped<StartExerciseSetValidator>();
        services.AddScoped<StartExerciseSetHandler>();
        services.AddScoped<SubmitAnswerValidator>();
        services.AddScoped<SubmitAnswerHandler>();

        return services;
    }
}