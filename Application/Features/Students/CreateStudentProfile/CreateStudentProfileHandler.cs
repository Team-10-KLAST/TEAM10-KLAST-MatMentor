using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain;

namespace Application.Features.Students.CreateStudentProfile;

public class CreateStudentProfileHandler
{
    private readonly ICreateStudentProfileRepository _repository;
    private readonly IIdentityService _identityService;

    public CreateStudentProfileHandler(ICreateStudentProfileRepository repository, IIdentityService identityService)
    {
        _repository = repository;
        _identityService = identityService;
    }

    public async Task HandleAsync(CreateStudentProfileRequest request)
    {
        var interestIds = (request.InterestIds ?? []).Distinct().ToList();
        var interests = await _repository.GetInterestsByIdAsync(interestIds);

        if (interests.Count != interestIds.Count)
        {
            var missing = interestIds.Except(interests.Select(i => i.Id));
            throw new NotFoundException(
                $"Interest(s) with IDs {string.Join(", ", missing)} do not exist.");
        }

        var student = new Student(request.ParentEmail, request.Grade, interests);

        var errors = await _identityService.CreateStudentWithLoginAsync(
            student, request.Username, request.Password);

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}
