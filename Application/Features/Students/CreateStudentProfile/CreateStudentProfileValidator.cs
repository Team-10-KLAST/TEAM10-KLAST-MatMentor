namespace Application.Features.Students.CreateStudentProfile;

using System.Net.Mail;

public class CreateStudentProfileValidator
{
    public List<string> Validate(CreateStudentProfileRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
            errors.Add("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            errors.Add("Password is required.");

        if (string.IsNullOrWhiteSpace(request.ParentEmail))
            errors.Add("Parent email is required.");
        else if (!MailAddress.TryCreate(request.ParentEmail, out _))
            errors.Add("Parent email is not a valid email address.");

        if (request.Grade < 7 || request.Grade > 9)
            errors.Add("Grade must be between 7 and 9.");

        return errors;
    }
}
