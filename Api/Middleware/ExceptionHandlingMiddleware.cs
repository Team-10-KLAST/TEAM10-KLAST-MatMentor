using Application.Common.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

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
        catch (AccountLockedException ex)
        {
            context.Response.StatusCode = 423;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }

        catch (OperationCanceledException ex) when (ex.InnerException is TimeoutException || !context.RequestAborted.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "The request to the AI service timed out.");
            context.Response.StatusCode = 504; // Gateway Timeout
            await context.Response.WriteAsJsonAsync(new { error = "The AI service took too long to respond. Please try again." });
        }

        catch (Exception ex)
        {
            // <--- HER LOGGER VI DEN NØJAGTIGE FEJL OG STACK TRACE TIL KONSOLLEN!
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }
    }
}