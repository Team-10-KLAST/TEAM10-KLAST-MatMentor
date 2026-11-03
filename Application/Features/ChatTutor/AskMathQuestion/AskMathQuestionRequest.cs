
namespace Application.Features.ChatTutor.AskMathQuestion;

public record AskMathQuestionRequest(
    string Question,
    string? StudentId
);