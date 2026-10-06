namespace Domain;

public class ExerciseSetAttempt
{
    public int Id { get; private set; }
    public int StudentId { get; private set; }
    public int ExerciseSetId { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public ExerciseSetAttempt(int studentId, int exerciseSetId)
    {
        StudentId = studentId;
        ExerciseSetId = exerciseSetId;
        StartedAt = DateTime.UtcNow;
    }
}
