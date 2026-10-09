namespace Domain;

public class AnswerSubmission
{
    public int Id { get; private set; }
    public int ExerciseSetAttemptId { get; private set; }
    public int ExerciseId { get; private set; }
    public string SubmittedAnswer { get; private set; }
    public DateTime AnsweredAt { get; private set; }
    public bool IsCorrect { get; private set; }

    public AnswerSubmission(int exerciseSetAttemptId, int exerciseId, string submittedAnswer, bool isCorrect)
    {
        ExerciseSetAttemptId = exerciseSetAttemptId;
        ExerciseId = exerciseId;
        SubmittedAnswer = submittedAnswer;
        IsCorrect = isCorrect;
        AnsweredAt = DateTime.UtcNow;
    }
}
