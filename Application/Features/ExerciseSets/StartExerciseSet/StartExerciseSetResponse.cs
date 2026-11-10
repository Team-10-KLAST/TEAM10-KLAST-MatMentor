namespace Application.Features.ExerciseSets.StartExerciseSet;

public record StartExerciseSetResponse(int AttemptId, int ExerciseSetId, string Title, IReadOnlyList<ExerciseItem> Exercises);
public record ExerciseItem(int Id, int Position, string ExerciseText, string? Unit);