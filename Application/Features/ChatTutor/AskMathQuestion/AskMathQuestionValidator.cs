
namespace Application.Features.ChatTutor.AskMathQuestion;

public class AskMathQuestionValidator
{
    public List<string> Validate(AskMathQuestionRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Question))
            errors.Add("Question is required.");

        return errors;
    }
}