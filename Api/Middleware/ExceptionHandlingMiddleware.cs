using Application.Common.Exceptions;

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
            await context.Response.WriteAsJsonAsync(new { errors = ex.Errors });
        }
        catch (UnauthorizedException ex)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
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