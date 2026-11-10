using Application.Common.Exceptions;
using System.Security.Claims;

namespace Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    // Returns the student id from the JWT claim, so it is never taken from the request body
    public static int GetStudentId(this ClaimsPrincipal user)
    {
        if (!int.TryParse(user.FindFirst("StudentId")?.Value, out int studentId) || studentId <= 0)
            throw new ForbiddenException("Student identity is missing.");

        return studentId;
    }
}