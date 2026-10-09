namespace Application.Features.ExerciseSets.StartExerciseSet;

public class StartExerciseSetValidator
{
    public List<string> Validate(StartExerciseSetRequest request)
    {
        var errors = new List<string>();

        if (request.StudentId <= 0)
        {
            errors.Add("StudentId must be a positive integer.");
        }
        if (request.TopicId <= 0)
        {
            errors.Add("TopicId must be a positive integer.");
        }
        return errors;
    }
}
