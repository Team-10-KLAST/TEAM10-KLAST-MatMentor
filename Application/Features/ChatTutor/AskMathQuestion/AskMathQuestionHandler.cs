using Application.Common.Exceptions;

namespace Application.Features.ChatTutor.AskMathQuestion;

public class AskMathQuestionHandler
{
    private readonly IAiMathService _aiMathService;

    public AskMathQuestionHandler(IAiMathService aiMathService)
    {
        _aiMathService = aiMathService;
    }

    public async Task<AskMathQuestionResponse> HandleAsync(AskMathQuestionRequest request)
    {
        var answer = await _aiMathService.GetExplanationAsync(request.Question);

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new ValidationException(["The AI tutor failed to generate a response."]);
        }

        return new AskMathQuestionResponse(answer);
    }
}