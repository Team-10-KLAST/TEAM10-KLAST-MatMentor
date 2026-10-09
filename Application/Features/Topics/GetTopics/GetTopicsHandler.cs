
namespace Application.Features.Topics.GetTopics;

public class GetTopicsHandler
{
    private readonly IGetTopicsRepository _repository;

    public GetTopicsHandler(IGetTopicsRepository repository)
        => _repository = repository;

    public async Task<GetTopicsResponse> HandleAsync()
    {
        var topics = await _repository.GetAllAsync();

        var items = topics
            .Select(t => new TopicItem(t.Id, t.Name))
            .ToList();

        return new GetTopicsResponse(items);
    }
}
