using Domain;

namespace Application.Common.Interfaces;

public interface IIdentityService
{
    // create a new student with login credentials
    Task<IReadOnlyList<string>> CreateStudentWithLoginAsync(
        Student student, string userName, string password);
}
