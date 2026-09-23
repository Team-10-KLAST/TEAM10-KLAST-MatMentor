using Domain;

namespace Application.Features.Students.CreateStudentProfile
{
    public interface ICreateStudentProfileRepository
    {
        Task AddAsync(Student student);
        Task<List<Interest>> GetInterestsByIdAsync(IEnumerable<int> interestIds);
    }
}