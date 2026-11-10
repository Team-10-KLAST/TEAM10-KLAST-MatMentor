using Application.Features.ChatTutor.Conversations;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.AI;

public sealed class TutorConversationCleanupService : BackgroundService
{
    private readonly ITutorConversationStore _store;

    public TutorConversationCleanupService(ITutorConversationStore store)
    {
        _store = store;
    }

    // Prunes idle server-side conversations once per minute without depending on incoming requests.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
        while (await timer.WaitForNextTickAsync(stoppingToken))
            _store.CleanupExpired();
    }
}
