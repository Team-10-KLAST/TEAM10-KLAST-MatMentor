using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Generates a signed JWT with the student's id as a claim and an expiry time from configuration
    public string GenerateToken(int studentId)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
        string secretKey = jwtSettings["SecretKey"]
            ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured");
        int expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]
            ?? throw new InvalidOperationException("JwtSettings:ExpiryMinutes is not configured"));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        Claim[] claims =
        [
            new Claim("StudentId", studentId.ToString())
        ];


        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
