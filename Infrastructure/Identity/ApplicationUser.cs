using Domain;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public int? StudentId { get; set; }
    public Student? Student { get; set; }
}
