using Application.Common.Exceptions;
using Application.Features.ChatTutor.AskTutor;
using Domain;

namespace Application.Features.ChatTutor.Conversations;

public sealed class TutorConversationHandler
{
    private readonly ITutorConversationStore _store;
    private readonly AskTutorHandler _askTutor;

    public TutorConversationHandler(ITutorConversationStore store, AskTutorHandler askTutor)
    {
        _store = store;
        _askTutor = askTutor;
    }

    // Creates an owned transient session and saves the first actual AI reply.
    public async Task<TutorConversationResponse> CreateAsync(
        int studentId, CreateTutorConversationRequest request, CancellationToken cancellationToken)
    {
        TutorConversation session = _store.Create(studentId, request.RequestId, request.ExerciseText!.Trim(), request.AttemptText!.Trim());
        if (!session.TurnGate.Wait(0))
            throw new TutorConflictException("Conversation is busy.");
        try
        {
            if (session.History.Count > 0)
                return new TutorConversationResponse(session.Id, session.History[0].Content, 0);
            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, session.LifetimeToken);
            AskTutorResponse response = await _askTutor.HandleAsync(
                new AskTutorRequest(session.ExerciseText, session.AttemptText, []), linked.Token);
            if (!_store.TryStoreInitialReply(session, response.Reply))
                throw new NotFoundException("Conversation has ended.");
            return new TutorConversationResponse(session.Id, response.Reply, session.Version);
        }
        catch (OperationCanceledException) when (session.LifetimeToken.IsCancellationRequested)
        {
            _store.Delete(session.Id, studentId);
            throw new NotFoundException("Conversation has ended.");
        }
        catch
        {
            _store.Delete(session.Id, studentId);
            throw;
        }
        finally
        {
            session.TurnGate.Release();
        }
    }

    // Serializes a student turn, rejects stale versions, and supports safe retries of the last accepted message.
    public async Task<TutorConversationResponse> SendAsync(
        int studentId, Guid conversationId, SendTutorMessageRequest request, CancellationToken cancellationToken)
    {
        TutorConversation session = _store.Find(conversationId, studentId)
            ?? throw new NotFoundException("Conversation not found or expired.");
        if (!session.TurnGate.Wait(0))
            throw new TutorConflictException("Conversation is already processing a message.");
        try
        {
            if (!_store.IsCurrent(session))
                throw new NotFoundException("Conversation not found or expired.");
            string text = request.Content!.Trim();
            string messageId = request.MessageId.ToString("N");
            if (session.HasAcceptedMessageId(messageId))
            {
                if (session.LastMessageId == messageId && session.LastStudentText == text
                    && request.ExpectedVersion == session.Version - 1 && session.LastTutorReply is not null)
                    return new TutorConversationResponse(session.Id, session.LastTutorReply, session.Version);
                throw new TutorConflictException("Message id has already been used.");
            }
            if (request.ExpectedVersion != session.Version)
                throw new TutorConflictException("Conversation version is out of date.");
            if (session.History.Count == 0)
                throw new TutorConflictException("Conversation is still starting.");
            if (session.History.Count + 2 > TutorConversation.MaxHistoryMessages
                || session.History.Sum(x => x.Content.Length) + text.Length >= TutorConversation.MaxHistoryCharacters)
                throw new ValidationException(["Conversation is full; start a new conversation."]);

            var history = session.History
                .Select(x => new AskTutorMessage(x.Role, x.Content))
                .Append(new AskTutorMessage("user", text))
                .ToArray();

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, session.LifetimeToken);
            AskTutorResponse response = await _askTutor.HandleAsync(
                new AskTutorRequest(session.ExerciseText, session.AttemptText, history), linked.Token);
            if (!_store.IsCurrent(session))
                throw new NotFoundException("Conversation has ended.");
            if (!session.CanAppend(text, response.Reply))
                throw new ValidationException(["Conversation is full; start a new conversation."]);
            if (!_store.TryStoreTurn(session, messageId, text, response.Reply))
                throw new NotFoundException("Conversation has ended.");
            return new TutorConversationResponse(session.Id, response.Reply, session.Version);
        }
        catch (OperationCanceledException) when (session.LifetimeToken.IsCancellationRequested)
        {
            throw new NotFoundException("Conversation has ended.");
        }
        finally
        {
            session.TurnGate.Release();
        }
    }

    // Returns only the authenticated owner's current conversation for safe recovery after lost responses.
    public TutorConversationStateResponse Get(int studentId, Guid conversationId)
    {
        TutorConversation session = _store.Find(conversationId, studentId)
            ?? throw new NotFoundException("Conversation not found or expired.");
        if (!session.TurnGate.Wait(0))
            throw new TutorConflictException("Conversation is busy.");
        try
        {
            if (!_store.IsCurrent(session))
                throw new NotFoundException("Conversation not found or expired.");
            return new TutorConversationStateResponse(session.Id, session.Version, session.History.ToArray());
        }
        finally
        {
            session.TurnGate.Release();
        }
    }

    // Closes only a session owned by the authenticated student, canceling any pending call.
    public bool End(int studentId, Guid conversationId)
    {
        return _store.Delete(conversationId, studentId);
    }
}
