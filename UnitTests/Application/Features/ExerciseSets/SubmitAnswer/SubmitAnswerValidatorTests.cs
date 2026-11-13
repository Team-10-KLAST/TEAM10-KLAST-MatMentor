using Application.Features.ExerciseSets.SubmitAnswer;

namespace UnitTests.Application.Features.ExerciseSets.SubmitAnswer;

public class SubmitAnswerValidatorTests
{
    // The app translates the backend's English messages to Danish, so the exact text matters
    private const string UseCommaMessage = "Use a comma as the decimal separator, not a period.";

    // An answer with a period gives a validation error telling the student to use a comma
    [Theory]
    [Trait("TestCase", "TC-010")]
    [InlineData("3.5")]
    [InlineData("1.000")]
    public void Validate_AnswerWithPeriod_ReturnsUseCommaError(string answer)
    {
        var validator = new SubmitAnswerValidator();
        var request = CreateRequestWithAnswer(answer);

        var errors = validator.Validate(request);

        var error = Assert.Single(errors);
        Assert.Equal(UseCommaMessage, error);
    }

    // Only the answer matters for these tests, so the ids are valid placeholders
    private static SubmitAnswerRequest CreateRequestWithAnswer(string answer)
        => new(ExerciseId: 1, Answer: answer);
}