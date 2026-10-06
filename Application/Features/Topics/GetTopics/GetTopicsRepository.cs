using Domain;

namespace Application.Features.Topics.GetTopics;

public interface IGetTopicsRepository
{
    Task<IReadOnlyList<Topic>> GetAllAsync();
}
