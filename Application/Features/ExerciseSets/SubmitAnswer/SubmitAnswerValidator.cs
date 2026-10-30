using Domain;

namespace Application.Features.ExerciseSets.SubmitAnswer;

public class SubmitAnswerValidator
{
    public List<string> Validate(SubmitAnswerRequest request)
    {
        var errors = new List<string>();

        if (request.AttemptId <= 0)
        {
            errors.Add("AttemptId must be a positive number.");
        }

        if (request.ExerciseId <= 0)
        {
            errors.Add("ExerciseId must be a positive number.");
        }

        if (string.IsNullOrWhiteSpace(request.Answer))
        {
            errors.Add("Answer is required.");
        }
        else if (AnswerFormat.ContainsPeriod(request.Answer))
        {
            errors.Add("Use a comma as the decimal separator, not a period.");
        }
        else if (!AnswerFormat.TryParse(request.Answer, out _))
        {
            errors.Add("Answer must be a number.");
        }

        return errors;
    }
}