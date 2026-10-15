namespace Application.Common.Interfaces;

public interface ITokenService
{
    // Generates a signed JWT containing the student's ID as a claim. The token is valid for 1 hour. 
    string GenerateToken(int studentId);
}
