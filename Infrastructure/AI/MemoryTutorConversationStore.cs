using Application.Common.Exceptions;
using Application.Features.ChatTutor.Conversations;
using Domain;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.AI;

public sealed class MemoryTutorConversationStore : ITutorConversationStore, IDisposable
{
    private const int MaxSessions = 128;
    private const int MaxSessionsPerStudent = 2;
    private static readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(30);
    private static readonly TimeSpan AbsoluteTimeout = TimeSpan.FromHours(4);

    private readonly object _sync = new();
    private readonly Dictionary<Guid, TutorConversation> _sessions = [];
    private readonly MemoryCache _cache;
    private readonly TimeProvider _clock;

    public MemoryTutorConversationStore(TimeProvider clock)
    {
        _clock = clock;
        _cache = new MemoryCache(new MemoryCacheOptions { SizeLimit = MaxSessions });
    }

    // Creates an isolated session with bounded cache size and at most two active sessions per student.
    public TutorConversation Create(int studentId, Guid startRequestId, string exerciseText, string attemptText)
    {
        lock (_sync)
        {
            PruneExpired();
            TutorConversation? previous = _sessions.Values.FirstOrDefault(x =>
                x.OwnerStudentId == studentId && x.StartRequestId == startRequestId);
            if (previous is not null)
            {
                if (previous.ExerciseText != exerciseText || previous.AttemptText != attemptText)
                    throw new ConflictException("The same request id was used with different content.");
                return previous;
            }
            if (_sessions.Values.Count(x => x.OwnerStudentId == studentId) >= MaxSessionsPerStudent)
                throw new LimitExceededException("Too many active conversations for this student.");
            if (_sessions.Count >= MaxSessions)
                throw new ServiceUnavailableException("The tutor has reached its conversation capacity.");
            var now = _clock.GetUtcNow();
            var session = new TutorConversation(Guid.NewGuid(), startRequestId, studentId, exerciseText, attemptText, now);
            _sessions.Add(session.Id, session);
            _cache.Set(session.Id, session, new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(IdleTimeout)
                .SetAbsoluteExpiration(AbsoluteTimeout)
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    if (state is MemoryTutorConversationStore store && value is TutorConversation expired)
                        store.OnEvicted(expired);
                }, this));
            if (!_cache.TryGetValue(session.Id, out _))
            {
                Remove(session);
                throw new ServiceUnavailableException("The tutor has reached its conversation capacity.");
            }
            return session;
        }
    }

    // Returns the session only for its JWT-authenticated owner, refreshing idle time on valid access.
    public TutorConversation? Find(Guid conversationId, int studentId)
    {
        lock (_sync)
        {
            if (!_sessions.TryGetValue(conversationId, out var session) || session.OwnerStudentId != studentId)
                return null;
            if (IsExpired(session) || !_cache.TryGetValue(conversationId, out _))
            {
                Remove(session);
                return null;
            }
            session.Touch(_clock.GetUtcNow());
            return session.IsClosed ? null : session;
        }
    }

    // Rejects late results from a session deleted, replaced, or expired during an AI request.
    public bool IsCurrent(TutorConversation session)
    {
        lock (_sync)
        {
            if (session.IsClosed || !_sessions.TryGetValue(session.Id, out var current)
                || !ReferenceEquals(current, session))
                return false;
            if (IsExpired(session) || !_cache.TryGetValue(session.Id, out _))
            {
                Remove(session);
                return false;
            }
            session.Touch(_clock.GetUtcNow());
            return true;
        }
    }

    // Atomically saves the initial reply only if the session has not been closed or evicted.
    public bool TryStoreInitialReply(TutorConversation session, string reply)
    {
        lock (_sync)
        {
            if (!IsCurrent(session)) return false;
            session.SetInitialReply(reply);
            return true;
        }
    }

    // Atomically saves a completed student and assistant exchange while preventing delete races.
    public bool TryStoreTurn(TutorConversation session, string messageId, string studentText, string reply)
    {
        lock (_sync)
        {
            if (!IsCurrent(session)) return false;
            session.Append(messageId, studentText, reply);
            return true;
        }
    }

    // Deletes only an owned session and cancels pending model calls without waiting for their completion.
    public bool Delete(Guid conversationId, int studentId)
    {
        lock (_sync)
        {
            if (!_sessions.TryGetValue(conversationId, out var session) || session.OwnerStudentId != studentId)
                return false;
            bool wasActive = !IsExpired(session);
            Remove(session);
            return wasActive;
        }
    }

    // Expires old sessions proactively even when no new requests arrive.
    public void CleanupExpired()
    {
        lock (_sync)
            PruneExpired();
    }

    // Removes sessions that are idle or older than the absolute lifetime before capacity checks.
    private void PruneExpired()
    {
        foreach (var session in _sessions.Values.Where(IsExpired).ToArray())
            Remove(session);
    }

    // Detects expiry even if the underlying cache has not yet run its background eviction scan.
    private bool IsExpired(TutorConversation session)
    {
        var now = _clock.GetUtcNow();
        return now - session.LastActivityUtc >= IdleTimeout || now - session.CreatedAtUtc >= AbsoluteTimeout;
    }

    // Removes both the cache entry and the index entry, invalidating any in-flight update.
    private void Remove(TutorConversation session)
    {
        session.Close();
        _sessions.Remove(session.Id);
        _cache.Remove(session.Id);
    }

    // Keeps the session index consistent with asynchronous cache evictions.
    private void OnEvicted(TutorConversation session)
    {
        lock (_sync)
        {
            if (_sessions.TryGetValue(session.Id, out var current) && ReferenceEquals(current, session))
            {
                session.Close();
                _sessions.Remove(session.Id);
            }
        }
    }

    // Disposes the isolated cache and invalidates all live sessions at service shutdown.
    public void Dispose()
    {
        lock (_sync)
        {
            foreach (var session in _sessions.Values)
                session.Close();
            _sessions.Clear();
            _cache.Dispose();
        }
    }
}
