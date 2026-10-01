namespace Domain;

public class Student
{
    private readonly List<Interest> _interests = new();

    public int Id { get; private set; }
    public string ParentEmail { get; private set; } = null!;
    public int Grade { get; private set; }
    public IReadOnlyCollection<Interest> Interests => _interests;

    protected Student() { }
    public Student(string parentEmail, int grade, IEnumerable<Interest> interests)
    {
        ParentEmail = parentEmail;
        Grade = grade;
        _interests.AddRange(interests);
    }

}
