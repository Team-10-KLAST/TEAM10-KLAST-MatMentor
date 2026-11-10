namespace Domain;

public class Student
{
    private readonly List<Interest> _interests = new();

    public int Id { get; private set; }
    public string ParentEmail { get; private set; } = null!;
    public int Grade { get; private set; }
    public IReadOnlyCollection<Interest> Interests => _interests;

    // Returns true when the student has chosen at least one interest
    public bool HasInterests => _interests.Count > 0;

    protected Student() { }
    public Student(string parentEmail, int grade, IEnumerable<Interest> interests)
    {
        ParentEmail = parentEmail;
        Grade = grade;
        _interests.AddRange(interests);
    }

}
