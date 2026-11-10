using Application.Common.Exceptions;
using Application.Features.ChatTutor.Conversations;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

    // Runs the rest of the pipeline and turns known exceptions into the matching HTTP status code
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new TutorValidationErrorResponse(ex.Errors));
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (TutorForbiddenException)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new TutorErrorResponse("Adgang nægtet."));
        }
        catch (TutorConflictException)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new TutorErrorResponse("Samtalen blev ændret eller er optaget. Prøv igen."));
        }
        catch (TutorLimitException)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new TutorErrorResponse("Du har for mange åbne samtaler. Afslut en først."));
        }
        catch (TutorTimeoutException)
        {
            context.Response.StatusCode = 504;
            await context.Response.WriteAsJsonAsync(
                new { error = "AI-tutoren svarede ikke i tide. Prøv igen." });
        }
        catch (TutorUnavailableException)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsJsonAsync(
                new { error = "AI-tutoren er midlertidigt utilgængelig. Prøv igen senere." });
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }
    }
}