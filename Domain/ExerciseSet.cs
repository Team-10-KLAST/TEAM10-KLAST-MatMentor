namespace Domain;

public class ExerciseSet
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public int Grade { get; private set; }
    public int? TopicId { get; private set; }
    public Topic? Topic { get; private set; }
    public int InterestId { get; private set; }
    public Interest Interest { get; private set; } = null!;
    private readonly List<Exercise> _exercises = new();
    public IReadOnlyList<Exercise> Exercises => _exercises;

    protected ExerciseSet() { } //For EF Core
    public ExerciseSet(string title, int grade, int? topicId, int interestId)
    {
        Title = title;
        Grade = grade;
        TopicId = topicId;
        InterestId = interestId;
    }
}
