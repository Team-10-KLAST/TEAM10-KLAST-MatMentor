using Domain;

namespace Application.Features.Students.CreateStudentProfile
{
    public interface ICreateStudentProfileRepository
    {
        Task<List<Interest>> GetInterestsByIdAsync(IReadOnlyCollection<int> interestIds);
    }
}