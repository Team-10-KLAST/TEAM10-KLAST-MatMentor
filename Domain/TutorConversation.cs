namespace Domain;

public sealed record TutorConversationMessage(string Role, string Content);

public sealed class TutorConversation
{
    public const int MaxMessageLength = 1500;
    public const int MaxHistoryCharacters = 12000;
    public const int MaxHistoryMessages = 21;

    private readonly List<TutorConversationMessage> _messages = [];
    private readonly HashSet<string> _acceptedMessageIds = new(StringComparer.Ordinal);
    private readonly CancellationTokenSource _lifetime = new();
    private int _closed;

    public Guid Id { get; }
    public Guid StartRequestId { get; }
    public int OwnerStudentId { get; }
    public string ExerciseText { get; }
    public string AttemptText { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset LastActivityUtc { get; private set; }
    public int Version { get; private set; }
    public string? LastMessageId { get; private set; }
    public string? LastStudentText { get; private set; }
    public string? LastTutorReply { get; private set; }
    public bool IsClosed => Volatile.Read(ref _closed) != 0;
    public CancellationToken LifetimeToken => _lifetime.Token;
    public SemaphoreSlim TurnGate { get; } = new(1, 1);
    public IReadOnlyList<TutorConversationMessage> History => _messages.AsReadOnly();

    public TutorConversation(Guid id, Guid startRequestId, int ownerStudentId, string exerciseText, string attemptText, DateTimeOffset createdAtUtc)
    {
        Id = id;
        StartRequestId = startRequestId;
        OwnerStudentId = ownerStudentId;
        ExerciseText = exerciseText;
        AttemptText = attemptText;
        CreatedAtUtc = createdAtUtc;
        LastActivityUtc = createdAtUtc;
    }

    // Refreshes the in-memory session's activity timestamp after an authorized request.
    public void Touch(DateTimeOffset nowUtc)
    {
        if (!IsClosed)
            LastActivityUtc = nowUtc;
    }

    // Stores only the AI's actual first reply after the model call succeeds.
    public void SetInitialReply(string reply)
    {
        EnsureActive();
        if (_messages.Count != 0 || string.IsNullOrWhiteSpace(reply) || reply.Length > MaxMessageLength)
            throw new InvalidOperationException("Invalid initial tutor response.");
        _messages.Add(new TutorConversationMessage("assistant", reply.Trim()));
    }

    // Checks that the next completed exchange fits within all conversation limits.
    public bool CanAppend(string studentText, string reply)
    {
        return !IsClosed && !string.IsNullOrWhiteSpace(studentText) && !string.IsNullOrWhiteSpace(reply)
            && studentText.Length <= MaxMessageLength && reply.Length <= MaxMessageLength
            && _messages.Count + 2 <= MaxHistoryMessages
            && _messages.Sum(x => x.Content.Length) + studentText.Length + reply.Length <= MaxHistoryCharacters;
    }

    // Returns whether this session already accepted the given student-message identifier.
    public bool HasAcceptedMessageId(string messageId) => _acceptedMessageIds.Contains(messageId);

    // Commits a complete exchange only after the model returned a valid reply.
    public void Append(string messageId, string studentText, string reply)
    {
        EnsureActive();
        if (!CanAppend(studentText, reply) || _messages.Count == 0)
            throw new InvalidOperationException("Conversation limit reached or invalid exchange.");
        if (string.IsNullOrWhiteSpace(messageId) || !_acceptedMessageIds.Add(messageId))
            throw new InvalidOperationException("Message id has already been accepted.");
        _messages.Add(new TutorConversationMessage("user", studentText));
        _messages.Add(new TutorConversationMessage("assistant", reply.Trim()));
        LastMessageId = messageId;
        LastStudentText = studentText;
        LastTutorReply = reply.Trim();
        Version++;
    }

    // Stops new changes and cancels any running model request.
    public void Close()
    {
        if (Interlocked.Exchange(ref _closed, 1) == 0)
            _lifetime.Cancel();
    }

    // Prevents closed sessions from being updated after a late model response.
    public void EnsureActive()
    {
        if (IsClosed)
            throw new InvalidOperationException("Conversation is closed.");
    }
}
