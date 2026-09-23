namespace Application.Features.Students.CreateStudentProfile;

public record CreateStudentProfileRequest(
    string Username,
    string Password,
    string ParentEmail,
    int Grade,
    List<int> Interests
);
