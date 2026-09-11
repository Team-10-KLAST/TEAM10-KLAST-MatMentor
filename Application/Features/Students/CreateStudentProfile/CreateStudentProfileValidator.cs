namespace Application.Features.Students.CreateStudentProfile;

public class CreateStudentProfileValidator
{
    public List<string> Validate(CreateStudentProfileRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Username))
            errors.Add("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            errors.Add("Password is required.");

        if (request.Grade < 7 || request.Grade > 9)
            errors.Add("Grade must be between 7 and 9.");

        return errors;
    }
}
