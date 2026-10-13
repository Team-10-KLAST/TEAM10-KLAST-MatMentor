using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public static class CompleteExerciseSetAttemptEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/exercise-sets/complete", async (
        CompleteExerciseSetAttemptRequest request,
        CompleteExerciseSetAttemptValidator validator,
        CompleteExerciseSetAttemptHandler handler) =>
        {
            var errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            await handler.HandleAsync(request);
            return Results.NoContent();

        });
    }
}