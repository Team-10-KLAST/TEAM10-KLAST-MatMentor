using Domain;

namespace Application.Features.ChatTutor.Conversations;

public interface ITutorConversationStore
{
    TutorConversation Create(int studentId, Guid startRequestId, string exerciseText, string attemptText);
    TutorConversation? Find(Guid conversationId, int studentId);
    bool IsCurrent(TutorConversation conversation);
    bool TryStoreInitialReply(TutorConversation session, string reply);
    bool TryStoreTurn(TutorConversation session, string messageId, string studentText, string reply);
    bool Delete(Guid conversationId, int studentId);
    void CleanupExpired();
}
