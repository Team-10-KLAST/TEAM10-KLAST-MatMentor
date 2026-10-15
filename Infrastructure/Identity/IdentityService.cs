using Application.Common.Interfaces;
using Application.Common.Models;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // Creates a login for the student and saves both together; returns the errors if creation fails
    public async Task<IReadOnlyList<string>> CreateStudentWithLoginAsync(
        Student student, string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Student = student
        };
        IdentityResult result = await _userManager.CreateAsync(user, password);

        return result.Succeeded
            ? []
            : result.Errors.Select(e => e.Description).ToList();
    }

    // Verifies the credentials with lockout enabled and returns the student's id on success
    public async Task<LoginResult> CheckCredentialsAsync(string userName, string password)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(userName);
        if (user is null || user.StudentId is null)
            return new LoginResult(false, false, null);

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(
            user, password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return new LoginResult(false, true, null);

        return result.Succeeded
            ? new LoginResult(true, false, user.StudentId)
            : new LoginResult(false, false, null);
    }
};
