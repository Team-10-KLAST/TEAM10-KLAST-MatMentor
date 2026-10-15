namespace Application.Features.Auth.Login;

public class LoginValidator
{
    // Checks that both username and password are provided
    public List<string> Validate(LoginRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
            errors.Add("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            errors.Add("Password is required.");

        return errors;
    }
}