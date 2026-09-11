using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Students.CreateStudentProfile;

public record CreateStudentProfileRequest(
    string Username,
    string Password,
    string ParentEmail,
    int Grade,
    int? InterestId
);
