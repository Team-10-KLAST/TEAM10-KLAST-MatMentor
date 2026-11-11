using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Application.Features.ExerciseSets.StartExerciseSet;

public static class StartExerciseSetEndpoint
{
    // Maps POST /api/exercise-set-attempts, which starts an exercise set for the logged-in student
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/exercise-set-attempts", async (
            StartExerciseSetRequest request,
            StartExerciseSetValidator validator,
            StartExerciseSetHandler handler,
            ClaimsPrincipal user) =>
        {
            List<string> errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            StartExerciseSetResponse response = await handler.HandleAsync(user.GetStudentId(), request);
            return TypedResults.Created($"/api/exercise-set-attempts/{response.AttemptId}", response);
        })
        .WithTags("Exercise Set Attempts")
        .Produces<StartExerciseSetResponse>(StatusCodes.Status201Created)
        .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }
}
