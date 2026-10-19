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
        catch (AccountLockedException ex)
        {
            context.Response.StatusCode = 423;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }
    }
}