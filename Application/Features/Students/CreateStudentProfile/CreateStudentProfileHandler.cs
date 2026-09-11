using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using Application.Common.Exceptions;

namespace Application.Features.Students.CreateStudentProfile;

public class CreateStudentProfileHandler
{
    private readonly ICreateStudentProfileRepository _repository;

    public CreateStudentProfileHandler(ICreateStudentProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(CreateStudentProfileRequest request)
    {
        Interest? interest = null;
        if (request.InterestId.HasValue)
        {
            interest = await _repository.GetInterestByIdAsync(request.InterestId.Value);
            if (interest == null)
            {
                throw new NotFoundException($"Interest with ID {request.InterestId.Value} does not exist.");
            }
        }
        var student = new Student(
            request.Username,
            request.Password,
            request.ParentEmail,
            request.Grade,
            interest
        );
        await _repository.AddAsync(student);
    }
}
