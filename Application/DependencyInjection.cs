using Microsoft.Extensions.DependencyInjection;
using Application.Features.Students.CreateStudentProfile;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateStudentProfileHandler>();
        services.AddScoped<CreateStudentProfileValidator>();   // ← ny linje
        return services;
    }
}