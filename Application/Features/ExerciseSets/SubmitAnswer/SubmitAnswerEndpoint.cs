using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.ExerciseSets.SubmitAnswer;

public static class SubmitAnswerEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/exercise-sets/submit-answer", async (
            SubmitAnswerRequest request,
            SubmitAnswerValidator validator,
            SubmitAnswerHandler handler) =>
        {
            var errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            await handler.HandleAsync(request);
            return Results.Ok();

        });
    }
}

