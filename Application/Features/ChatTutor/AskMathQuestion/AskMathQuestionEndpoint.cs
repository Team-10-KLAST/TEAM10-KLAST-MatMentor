using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.ChatTutor.AskMathQuestion;

public static class AskMathQuestionEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chat-tutor/ask", async (
            AskMathQuestionRequest request,
            AskMathQuestionValidator validator,
            AskMathQuestionHandler handler) =>
        {
            var errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            var response = await handler.HandleAsync(request);
            return Results.Ok(response);
        });
    }
}