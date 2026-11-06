namespace Application.Features.ChatTutor.AskTutor;

public sealed class AskTutorValidator
{
    public const int MaxExerciseLength = 4000;
    public const int MaxAttemptLength = 2000;
    public const int MaxMessageLength = 1500;
    public const int MaxMessageCount = 20;
    public const int MaxHistoryLength = 12000;

    // Validates the stateless tutor request and its ordered, client-supplied conversation history.
    public List<string> Validate(AskTutorRequest? request)
    {
        var errors = new List<string>();
        if (request is null)
        {
            errors.Add("Request is required.");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(request.ExerciseText))
            errors.Add("ExerciseText is required.");
        else if (request.ExerciseText.Length > MaxExerciseLength)
            errors.Add("ExerciseText is too long.");

        if (string.IsNullOrWhiteSpace(request.AttemptText))
            errors.Add("AttemptText is required.");
        else if (request.AttemptText.Length > MaxAttemptLength)
            errors.Add("AttemptText is too long.");

        var messages = request.Messages;
        if (messages is null)
        {
            errors.Add("Messages is required; use an empty array for a new conversation.");
            return errors;
        }

        if (messages.Count > MaxMessageCount)
            errors.Add("Conversation history contains too many messages.");

        if (messages.Count % 2 != 0)
            errors.Add("Conversation history must contain complete assistant/user exchanges.");

        long totalLength = 0;
        for (var i = 0; i < messages.Count; i++)
        {
            AskTutorMessage? message = messages[i];
            if (message is null)
            {
                errors.Add("Conversation history contains an invalid message.");
                continue;
            }

            string expectedRole = i % 2 == 0 ? "assistant" : "user";
            if (!string.Equals(message.Role, expectedRole, StringComparison.Ordinal))
                errors.Add($"Message {i + 1} must have role '{expectedRole}'.");

            if (string.IsNullOrWhiteSpace(message.Content))
                errors.Add($"Message {i + 1} is empty.");
            else if (message.Content.Length > MaxMessageLength)
                errors.Add($"Message {i + 1} is too long.");

            totalLength += message.Content?.Length ?? 0;
        }

        if (totalLength > MaxHistoryLength)
            errors.Add("Conversation history is too long.");

        return errors;
    }
}
