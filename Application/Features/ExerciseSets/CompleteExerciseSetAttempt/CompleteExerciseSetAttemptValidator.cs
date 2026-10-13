namespace Application.Features.ExerciseSets.CompleteExerciseSetAttempt;

public class CompleteExerciseSetAttemptValidator
{
    public List<string> Validate(CompleteExerciseSetAttemptRequest request)
    {
        var errors = new List<string>();

        if (request.AttemptId <= 0)
        {
            errors.Add("AttemptId must be a positive integer.");
        }
        return errors;
    }
}