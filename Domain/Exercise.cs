namespace Domain;

public class Exercise
{
    public int Id { get; private set; }
    public int Position { get; private set; }
    public string ExerciseText { get; private set; }
    public string CorrectAnswer { get; private set; }
    public string? Unit { get; private set; }
    public int ExerciseSetId { get; private set; }
    public int TopicId { get; private set; }

    //Compares the student's answer to correct answer.
    //Trims whitespace and ignores casing.
    public bool IsAnswerCorrect(string submittedAnswer)
        => string.Equals(CorrectAnswer, submittedAnswer, StringComparison.OrdinalIgnoreCase);

    public Exercise(int position, string exerciseText, string correctAnswer, string? unit, int exerciseSetId, int topicId)
    {
        Position = position;
        ExerciseText = exerciseText;
        CorrectAnswer = correctAnswer;
        Unit = unit;
        ExerciseSetId = exerciseSetId;
        TopicId = topicId;
    }
}
