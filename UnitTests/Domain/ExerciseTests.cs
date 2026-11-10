using Domain;

namespace UnitTests.Domain;

public class ExerciseTests
{
    // The same number written differently is judged correct
    [Theory]
    [Trait("TestCase", "TC-008")]
    [InlineData("3,50", "3,5")]
    [InlineData("3", "3,0")]
    public void IsAnswerCorrect_SameNumberWrittenDifferently_ReturnsTrue(string submittedAnswer, string correctAnswer)
    {
        var exercise = CreateExerciseWithCorrectAnswer(correctAnswer);

        var isCorrect = exercise.IsAnswerCorrect(submittedAnswer);

        Assert.True(isCorrect);
    }

    // A different number is judged wrong
    [Fact]
    [Trait("TestCase", "TC-009")]
    public void IsAnswerCorrect_DifferentNumber_ReturnsFalse()
    {
        var exercise = CreateExerciseWithCorrectAnswer("3,5");
        var submittedAnswer = "4";

        var isCorrect = exercise.IsAnswerCorrect(submittedAnswer);

        Assert.False(isCorrect);
    }

    // Only the correct answer matters for these tests, so the other values are placeholders
    private static Exercise CreateExerciseWithCorrectAnswer(string correctAnswer)
        => new(position: 1, exerciseText: "Test exercise", correctAnswer: correctAnswer, unit: null, exerciseSetId: 1, topicId: 1);
}
