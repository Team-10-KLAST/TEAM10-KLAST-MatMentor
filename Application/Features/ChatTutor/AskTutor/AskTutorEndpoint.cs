using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.ChatTutor.AskTutor;

public static class AskTutorEndpoint
{
    // Maps the authenticated tutor endpoint and enforces its per-student rate limit.
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/chat-tutor/ask", async (
            AskTutorRequest request,
            AskTutorValidator validator,
            AskTutorHandler handler,
            CancellationToken cancellationToken) =>
        {
            List<string> errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            AskTutorResponse reply = await handler.HandleAsync(request, cancellationToken);
            return Results.Ok(reply);
        })
        .RequireAuthorization()
        .RequireRateLimiting("TutorRateLimit");
    }
}
