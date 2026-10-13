using Application.Features.ExerciseSets.StartExerciseSet;
using Application.Features.ExerciseSets.SubmitAnswer;
using Application.Features.Students.CreateStudentProfile;
using Application.Features.Topics.GetTopics;
using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateStudentProfileHandler>();
        services.AddScoped<CreateStudentProfileValidator>();
        services.AddScoped<GetTopicsHandler>();
        services.AddScoped<StartExerciseSetValidator>();
        services.AddScoped<StartExerciseSetHandler>();
        services.AddScoped<SubmitAnswerValidator>();
        services.AddScoped<SubmitAnswerHandler>();
        services.AddScoped<CompleteExerciseSetAttemptValidator>();
        services.AddScoped<CompleteExerciseSetAttemptHandler>();

        return services;
    }
}