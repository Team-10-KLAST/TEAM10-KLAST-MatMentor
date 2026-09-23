using Application.Common.Exceptions;
using Domain;

namespace Application.Features.Students.CreateStudentProfile;

public class CreateStudentProfileHandler
{
    private readonly ICreateStudentProfileRepository _repository;

    public CreateStudentProfileHandler(ICreateStudentProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateStudentProfileResponse> HandleAsync(CreateStudentProfileRequest request)
    {
        var interests = await _repository.GetInterestsByIdAsync(request.Interests);

        var missingIds = request.Interests.Except(interests.Select(i => i.Id)).ToList();
        if (missingIds.Count > 0)
        {
            throw new NotFoundException($"Interest(s) with ID {string.Join(", ", missingIds)} do not exist.");
        }

        var student = new Student(request.Username, request.Password, request.ParentEmail, request.Grade, interests);
        await _repository.AddAsync(student);

        return new CreateStudentProfileResponse(student.Id);
    }
}
