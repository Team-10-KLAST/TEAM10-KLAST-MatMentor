using Application.Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Auth.Login;

public static class LoginEndpoint
{
    // Maps POST /api/auth/login, which validates the request and returns a JWT on successful login
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            LoginValidator validator,
            LoginHandler handler) =>
        {
            List<string> errors = validator.Validate(request);
            if (errors.Count > 0)
                throw new ValidationException(errors);

            LoginResponse response = await handler.HandleAsync(request);
            return Results.Ok(response);
        })
        .AllowAnonymous();
    }
}