using Application.Common.Extensions;
using Application.Common.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public static class CompleteExerciseSetAttemptEndpoint
{
    // Maps POST /api/exercise-set-attempts/{attemptId}/completion, which completes the student's own attempt
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/exercise-set-attempts/{attemptId:int}/completion", async (
            int attemptId,
            CompleteExerciseSetAttemptHandler handler,
            ClaimsPrincipal user) =>
        {
            await handler.HandleAsync(user.GetStudentId(), attemptId);
            return TypedResults.NoContent();
        })
        .WithTags("Exercise Set Attempts")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict);
    }
}