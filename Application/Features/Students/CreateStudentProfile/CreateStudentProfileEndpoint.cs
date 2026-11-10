using Application.Common.Exceptions;
using Application.Common.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Students.CreateStudentProfile;

public static class CreateStudentProfileEndpoint
{
    // Maps POST /api/students, which validates the request and creates a student profile with login
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/students", async (
            CreateStudentProfileRequest request,
            CreateStudentProfileValidator validator,
            CreateStudentProfileHandler handler) =>
        {
            List<string> errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            await handler.HandleAsync(request);
            return TypedResults.Created();
        })
        .AllowAnonymous()
        .WithTags("Students")
        .Produces(StatusCodes.Status201Created)
        .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}