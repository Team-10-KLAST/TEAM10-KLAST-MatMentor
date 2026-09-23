using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Students.CreateStudentProfile;

public static class CreateStudentProfileEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/students", async (
            CreateStudentProfileRequest request,
            CreateStudentProfileValidator validator,
            CreateStudentProfileHandler handler) =>
        {
            var errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            var response = await handler.HandleAsync(request);
            return Results.Created($"/api/students/{response.Id}", response);
        });
    }
}