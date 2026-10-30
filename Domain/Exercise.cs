
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

    //Compares the answers as numbers, not as text. Otherwise 1,5 and 1,50 would differ.
    public bool IsAnswerCorrect(string submittedAnswer)
    {
        if (!AnswerFormat.TryParse(submittedAnswer, out var submitted))
            return false;

        if (!AnswerFormat.TryParse(CorrectAnswer, out var correct))
            throw new InvalidOperationException(
                $"Exercise {Id} has a correct answer that is not a valid number: '{CorrectAnswer}'.");

        return submitted == correct;
    }

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