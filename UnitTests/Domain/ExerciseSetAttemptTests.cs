using Domain;

namespace UnitTests.Domain;

public class ExerciseSetAttemptTests
{
    // An attempt that is already completed cannot be completed again
    [Fact]
    [Trait("TestCase", "TC-021")]
    public void Complete_AttemptAlreadyCompleted_ThrowsInvalidOperationException()
    {
        var attempt = new ExerciseSetAttempt(studentId: 1, exerciseSetId: 1);
        attempt.Complete();

        var exception = Record.Exception(() => attempt.Complete());

        Assert.IsType<InvalidOperationException>(exception);
    }
}
