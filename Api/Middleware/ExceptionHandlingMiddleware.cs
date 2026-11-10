using Application.Common.Exceptions;
using Application.Common.Responses;

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
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ValidationErrorResponse(ex.Errors));
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
        catch (ForbiddenException)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new ErrorResponse("Adgang nægtet."));
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new ErrorResponse(ex.Message));
        }
        catch (LimitExceededException)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsJsonAsync(new ErrorResponse("Du har for mange åbne samtaler. Afslut en først."));
        }
        catch (ServiceTimeoutException)
        {
            context.Response.StatusCode = 504;
            await context.Response.WriteAsJsonAsync(
                new { error = "AI-tutoren svarede ikke i tide. Prøv igen." });
        }
        catch (ServiceUnavailableException)
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