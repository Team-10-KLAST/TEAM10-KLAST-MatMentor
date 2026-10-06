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
        return errors;
    }
}
