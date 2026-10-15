using Application.Common.Models;
using Domain;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    // creates a new student with login credentials and returns any errors
    Task<IReadOnlyList<string>> CreateStudentWithLoginAsync(
        Student student, string userName, string password);

    // Checks username and password and reports whether the login succeeded or the account is locked out
    Task<LoginResult> CheckCredentialsAsync(string userName, string password);
}
