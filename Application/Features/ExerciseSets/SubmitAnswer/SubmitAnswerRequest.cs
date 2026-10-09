namespace Application.Features.ExerciseSets.SubmitAnswer;

public record SubmitAnswerRequest(int AttemptId, int ExerciseId, string Answer);
