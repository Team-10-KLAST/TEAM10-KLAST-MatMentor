using Application.Common.Interfaces;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<string>> CreateStudentWithLoginAsync(
        Student student, string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Student = student
        };
        var result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? []
            : result.Errors.Select(e => e.Description).ToList();
    }
};
