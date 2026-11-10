using Application.Features.ChatTutor.AskTutor;
using Domain;

namespace Application.Features.ChatTutor.Conversations;

public sealed class TutorConversationValidator
{
    // Validates the initial task and attempt without trusting client-supplied conversation history.
    public List<string> ValidateCreate(CreateTutorConversationRequest? request)
    {
        var errors = new List<string>();
        if (request is null)
        {
            errors.Add("Request is required.");
            return errors;
        }
        if (string.IsNullOrWhiteSpace(request.ExerciseText))
            errors.Add("ExerciseText is required.");
        else if (request.ExerciseText.Length > AskTutorValidator.MaxExerciseLength)
            errors.Add("ExerciseText is too long.");
        if (string.IsNullOrWhiteSpace(request.AttemptText))
            errors.Add("AttemptText is required.");
        else if (request.AttemptText.Length > AskTutorValidator.MaxAttemptLength)
            errors.Add("AttemptText is too long.");
        if (request.RequestId == Guid.Empty)
            errors.Add("RequestId is required.");
        return errors;
    }

    // Validates a new student turn, its version, and its client-generated idempotency identifier.
    public List<string> ValidateMessage(SendTutorMessageRequest? request)
    {
        var errors = new List<string>();
        if (request is null)
        {
            errors.Add("Request is required.");
            return errors;
        }
        if (string.IsNullOrWhiteSpace(request.Content))
            errors.Add("Content is required.");
        else if (request.Content.Length > TutorConversation.MaxMessageLength)
            errors.Add("Content is too long.");
        if (request.ExpectedVersion < 0)
            errors.Add("ExpectedVersion must be non-negative.");
        if (request.MessageId == Guid.Empty)
            errors.Add("MessageId is required.");
        return errors;
    }
}
