using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace Application.Features.ChatTutor.Conversations;

public static class TutorConversationEndpoints
{
    // Maps resource-oriented and authenticated conversation endpoints with OpenAPI response metadata.
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tutor-conversations", async (
            CreateTutorConversationRequest request,
            TutorConversationValidator validator,
            TutorConversationHandler handler,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            List<string> errors = validator.ValidateCreate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);
            int studentId = GetStudentId(context.User);
            TutorConversationResponse response = await handler.CreateAsync(studentId, request, cancellationToken);
            return TypedResults.Created($"/api/tutor-conversations/{response.ConversationId}", response);
        })
        .WithTags("Tutor Conversations")
        .Produces<TutorConversationResponse>(StatusCodes.Status201Created)
        .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status429TooManyRequests)
        .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)
        .Produces<ErrorResponse>(StatusCodes.Status504GatewayTimeout)
        .RequireRateLimiting("TutorRateLimit");

        app.MapPost("/api/tutor-conversations/{conversationId:guid}/messages", async (
            Guid conversationId,
            SendTutorMessageRequest request,
            TutorConversationValidator validator,
            TutorConversationHandler handler,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            List<string> errors = validator.ValidateMessage(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);
            int studentId = GetStudentId(context.User);
            TutorConversationResponse response = await handler.SendAsync(studentId, conversationId, request, cancellationToken);
            return TypedResults.Ok(response);
        })
        .WithTags("Tutor Conversations")
        .Produces<TutorConversationResponse>(StatusCodes.Status200OK)
        .Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status429TooManyRequests)
        .Produces<ErrorResponse>(StatusCodes.Status503ServiceUnavailable)
        .Produces<ErrorResponse>(StatusCodes.Status504GatewayTimeout)
        .RequireRateLimiting("TutorRateLimit");

        app.MapGet("/api/tutor-conversations/{conversationId:guid}", (
            Guid conversationId,
            TutorConversationHandler handler,
            HttpContext context) =>
        {
            int studentId = GetStudentId(context.User);
            return TypedResults.Ok(handler.Get(studentId, conversationId));
        })
        .WithTags("Tutor Conversations")
        .Produces<TutorConversationStateResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces<ErrorResponse>(StatusCodes.Status409Conflict)
        .Produces<ErrorResponse>(StatusCodes.Status429TooManyRequests)
        .RequireRateLimiting("TutorReadRateLimit");

        app.MapDelete("/api/tutor-conversations/{conversationId:guid}", (
            Guid conversationId,
            TutorConversationHandler handler,
            HttpContext context) =>
        {
            int studentId = GetStudentId(context.User);
            if (!handler.End(studentId, conversationId))
                throw new NotFoundException("Conversation not found or expired.");
            return TypedResults.NoContent();
        })
        .WithTags("Tutor Conversations")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces<ErrorResponse>(StatusCodes.Status403Forbidden)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    // Extracts a validated student identity from the JWT claim, independent of client-provided data.
    private static int GetStudentId(ClaimsPrincipal user)
    {
        if (!int.TryParse(user.FindFirst("StudentId")?.Value, out int studentId) || studentId <= 0)
            throw new TutorForbiddenException("Student identity is missing.");
        return studentId;
    }
}
