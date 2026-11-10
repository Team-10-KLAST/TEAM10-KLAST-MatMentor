namespace Application.Features.ChatTutor.AskTutor;

public sealed record AskTutorRequest(
    string? ExerciseText,
    string? AttemptText,
    IReadOnlyList<AskTutorMessage>? Messages);
