namespace Application.Features.ChatTutor.Conversations;

public sealed record CreateTutorConversationRequest(string? ExerciseText, string? AttemptText, Guid RequestId);
public sealed record SendTutorMessageRequest(string? Content, int ExpectedVersion, Guid MessageId);
public sealed record TutorConversationResponse(Guid ConversationId, string Reply, int Version);
public sealed record TutorConversationStateResponse(Guid ConversationId, int Version, IReadOnlyList<Domain.TutorConversationMessage> Messages);
public sealed record ErrorResponse(string Error);
public sealed record ValidationErrorResponse(IReadOnlyList<string> Errors);
