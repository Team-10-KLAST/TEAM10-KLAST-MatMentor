namespace Domain;

public class ExerciseSet
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public int Grade { get; private set; }
    public int? TopicId { get; private set; }
    public Topic? Topic { get; private set; }
    public int? InterestId { get; private set; }
    public Interest? Interest { get; private set; }
    private readonly List<Exercise> _exercises = new();
    public IReadOnlyList<Exercise> Exercises => _exercises;

    protected ExerciseSet() { } //For EF Core
    public ExerciseSet(string title, int grade, int? topicId, int? interestId)
    {
        Title = title;
        Grade = grade;
        TopicId = topicId;
        InterestId = interestId;
    }

    // Adds an exercise to the set. EF Core sets ExerciseSetId when the set is saved
    public void AddExercise(int position, string exerciseText, string correctAnswer, string? unit, int topicId)
    {
        _exercises.Add(new Exercise(position, exerciseText, correctAnswer, unit, Id, topicId));
    }
}
