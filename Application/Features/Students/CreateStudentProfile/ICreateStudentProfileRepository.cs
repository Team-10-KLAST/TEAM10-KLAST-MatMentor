using Domain;

namespace Application.Features.Students.CreateStudentProfile
{
    public interface ICreateStudentProfileRepository
    {
        Task AddAsync(Student student);
        Task<Interest?> GetInterestByIdAsync(int interestId);
    }
}