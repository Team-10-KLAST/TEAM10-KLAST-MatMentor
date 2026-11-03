

namespace Application.Features.ChatTutor.AskMathQuestion;

public interface IAiMathService
{
    Task<string> GetExplanationAsync(string question);
}