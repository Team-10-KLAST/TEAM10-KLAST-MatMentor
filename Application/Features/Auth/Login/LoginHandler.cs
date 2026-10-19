using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;

namespace Application.Features.Auth.Login;

public class LoginHandler
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginHandler(IIdentityService identityService, ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    // Verifies the credentials and returns a JWT for the student, or throws if login is not allowed
    public async Task<LoginResponse> HandleAsync(LoginRequest request)
    {
        LoginResult result = await _identityService.CheckCredentialsAsync(
            request.Username, request.Password);

        if (result.IsLockedOut)
            throw new AccountLockedException("The account is temporarily locked. Try again later.");

        if (!result.Succeeded || result.StudentId is null)
            throw new UnauthorizedException("Invalid username or password.");

        string token = _tokenService.GenerateToken(result.StudentId.Value);
        return new LoginResponse(token);
    }
}