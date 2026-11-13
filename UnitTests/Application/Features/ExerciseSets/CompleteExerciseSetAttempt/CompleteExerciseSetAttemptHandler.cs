using Application.Common.Exceptions;
using Application.Features.ExerciseSets.CompleteExerciseSetAttempt;
using Domain;

namespace UnitTests.Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public class CompleteExerciseSetAttemptHandlerTests
{
    private const int StudentId = 1;
    private const int OtherStudentId = 2;
    private const int AttemptId = 7;
    private const int UnknownAttemptId = 99;

    // An active attempt is completed
    [Fact]
    [Trait("TestCase", "TC-018")]
    public async Task HandleAsync_ActiveAttempt_CompletesAttempt()
    {
        var attempt = CreateAttempt();
        var repository = new FakeCompleteExerciseSetAttemptRepository();
        repository.AddAttempt(AttemptId, attempt);
        var handler = new CompleteExerciseSetAttemptHandler(repository);

        await handler.HandleAsync(StudentId, AttemptId);

        Assert.NotNull(attempt.CompletedAt);
    }

    // An attempt that is already completed cannot be completed again
    [Fact]
    [Trait("TestCase", "TC-019")]
    public async Task HandleAsync_AttemptAlreadyCompleted_ThrowsConflictException()
    {
        var attempt = CreateAttempt();
        attempt.Complete();
        var repository = new FakeCompleteExerciseSetAttemptRepository();
        repository.AddAttempt(AttemptId, attempt);
        var handler = new CompleteExerciseSetAttemptHandler(repository);

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(StudentId, AttemptId));

        Assert.IsType<ConflictException>(exception);
    }

    // An unknown attempt and another student's attempt give the same error, so it cannot be seen whether the attempt exists
    [Theory]
    [Trait("TestCase", "TC-020")]
    [InlineData(UnknownAttemptId, StudentId)]
    [InlineData(AttemptId, OtherStudentId)]
    public async Task HandleAsync_UnknownAttemptOrOtherStudentsAttempt_ThrowsNotFoundException(int requestedAttemptId, int requestingStudentId)
    {
        var repository = new FakeCompleteExerciseSetAttemptRepository();
        repository.AddAttempt(AttemptId, CreateAttempt());
        var handler = new CompleteExerciseSetAttemptHandler(repository);

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(requestingStudentId, requestedAttemptId));

        Assert.IsType<NotFoundException>(exception);
    }

    // The attempt belongs to StudentId. The exercise set does not matter for these tests
    private static ExerciseSetAttempt CreateAttempt()
        => new(StudentId, exerciseSetId: 1);
}