using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Responses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Application.Features.ExerciseSets.SubmitAnswer;

public static class SubmitAnswerEndpoint
{
    // Maps POST /api/exercise-set-attempts/{attemptId}/answers, which saves an answer in the student's own attempt
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/exercise-set-attempts/{attemptId:int}/answers", async (
            int attemptId,
            SubmitAnswerRequest request,
            SubmitAnswerValidator validator,
            SubmitAnswerHandler handler,
            ClaimsPrincipal user) =>
        {
            List<string> errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            await handler.HandleAsync(user.GetStudentId(), attemptId, request);
            return TypedResults.NoContent();
        })
        .WithTags("Exercise Set Attempts")
        .Produces(StatusCodes.Status204NoContent)
        .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict);
    }
}

