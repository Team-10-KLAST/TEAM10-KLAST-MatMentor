using Application.Common.Exceptions;
using Application.Features.ExerciseSets.SubmitAnswer;
using Domain;

namespace UnitTests.Application.Features.ExerciseSets.SubmitAnswer;

public class SubmitAnswerHandlerTests
{
    private const int StudentId = 1;
    private const int OtherStudentId = 2;
    private const int AttemptId = 7;
    private const int UnknownAttemptId = 99;
    private const int ExerciseId = 3;
    private const int ExerciseSetId = 10;
    private const int OtherExerciseSetId = 20;
    private const string CorrectAnswer = "3,5";

    // An answer cannot be added to an attempt that is already completed
    [Fact]
    [Trait("TestCase", "TC-013")]
    public async Task HandleAsync_AttemptAlreadyCompleted_ThrowsConflictException()
    {
        var attempt = CreateAttempt();
        attempt.Complete();
        var repository = new FakeSubmitAnswerRepository();
        repository.AddAttempt(AttemptId, attempt);
        var handler = new SubmitAnswerHandler(repository);
        var request = CreateRequest();

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(StudentId, AttemptId, request));

        Assert.IsType<ConflictException>(exception);
    }

    // An exercise from another exercise set cannot be answered in the attempt
    [Fact]
    [Trait("TestCase", "TC-014")]
    public async Task HandleAsync_ExerciseFromAnotherSet_ThrowsValidationException()
    {
        var repository = new FakeSubmitAnswerRepository();
        repository.AddAttempt(AttemptId, CreateAttempt());
        repository.AddExercise(ExerciseId, CreateExerciseInSet(OtherExerciseSetId));
        var handler = new SubmitAnswerHandler(repository);
        var request = CreateRequest();

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(StudentId, AttemptId, request));

        Assert.IsType<ValidationException>(exception);
    }

    // The same exercise cannot be answered twice in one attempt
    [Fact]
    [Trait("TestCase", "TC-015")]
    public async Task HandleAsync_ExerciseAlreadyAnswered_ThrowsConflictException()
    {
        var attempt = CreateAttempt();
        var exercise = CreateExerciseInSet(ExerciseSetId);
        var repository = new FakeSubmitAnswerRepository();
        repository.AddAttempt(AttemptId, attempt);
        repository.AddExercise(ExerciseId, exercise);
        repository.Submissions.Add(new AnswerSubmission(attempt.Id, exercise.Id, "3,5", true));
        var handler = new SubmitAnswerHandler(repository);
        var request = CreateRequest();

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(StudentId, AttemptId, request));

        Assert.IsType<ConflictException>(exception);
    }

    // An unknown attempt and another student's attempt give the same error, so it cannot be seen whether the attempt exists
    [Theory]
    [Trait("TestCase", "TC-016")]
    [InlineData(UnknownAttemptId, StudentId)]
    [InlineData(AttemptId, OtherStudentId)]
    public async Task HandleAsync_UnknownAttemptOrOtherStudentsAttempt_ThrowsNotFoundException(int requestedAttemptId, int requestingStudentId)
    {
        var repository = new FakeSubmitAnswerRepository();
        repository.AddAttempt(AttemptId, CreateAttempt());
        var handler = new SubmitAnswerHandler(repository);
        var request = CreateRequest();

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(requestingStudentId, requestedAttemptId, request));

        Assert.IsType<NotFoundException>(exception);
    }

    // An answer to an exercise that does not exist is rejected
    [Fact]
    [Trait("TestCase", "TC-017")]
    public async Task HandleAsync_UnknownExercise_ThrowsNotFoundException()
    {
        var repository = new FakeSubmitAnswerRepository();
        repository.AddAttempt(AttemptId, CreateAttempt());
        var handler = new SubmitAnswerHandler(repository);
        var request = CreateRequest();

        var exception = await Record.ExceptionAsync(() => handler.HandleAsync(StudentId, AttemptId, request));

        Assert.IsType<NotFoundException>(exception);
    }

    // A valid answer is saved once, with the student's answer and whether it is correct
    [Theory]
    [Trait("TestCase", "TC-025")]
    [InlineData(CorrectAnswer, true)]
    [InlineData("4", false)]
    public async Task HandleAsync_ValidAnswer_SavesOneSubmissionWithAnswerAndAssessment(string answer, bool expectedIsCorrect)
    {
        var repository = new FakeSubmitAnswerRepository();
        repository.AddAttempt(AttemptId, CreateAttempt());
        repository.AddExercise(ExerciseId, CreateExerciseInSet(ExerciseSetId));
        var handler = new SubmitAnswerHandler(repository);
        var request = new SubmitAnswerRequest(ExerciseId: ExerciseId, Answer: answer);

        await handler.HandleAsync(StudentId, AttemptId, request);

        var submission = Assert.Single(repository.Submissions);
        Assert.Equal(answer, submission.SubmittedAnswer);
        Assert.Equal(expectedIsCorrect, submission.IsCorrect);
    }

    // The attempt belongs to StudentId and is started for ExerciseSetId
    private static ExerciseSetAttempt CreateAttempt()
        => new(StudentId, ExerciseSetId);

    // Only the exercise set and the correct answer matter for these tests, so the other values are placeholders
    private static Exercise CreateExerciseInSet(int exerciseSetId)
        => new(position: 1, exerciseText: "Test exercise", correctAnswer: CorrectAnswer, unit: null, exerciseSetId: exerciseSetId, topicId: 1);

    private static SubmitAnswerRequest CreateRequest()
        => new(ExerciseId: ExerciseId, Answer: "3,5");
}
