using Application.Features.Topics.GetTopics;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Topics;

public class TopicRepository : IGetTopicsRepository
{
    private readonly AppDbContext _db;

    public TopicRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Topic>> GetAllAsync()
        => await _db.Topics
            .OrderBy(t => t.Name)
            .ToListAsync();
}