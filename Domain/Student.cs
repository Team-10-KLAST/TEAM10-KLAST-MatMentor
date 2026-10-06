namespace Domain;

public class Student
{
    private readonly List<Interest> _interests = new();

    public int Id { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public string ParentEmail { get; private set; } = null!;
    public int Grade { get; private set; }
    public IReadOnlyCollection<Interest> Interests => _interests;

    protected Student() { } // To EF Core
    public Student(string userName, string password, string parentEmail, int grade, Interest? interest)
    {
        ParentEmail = parentEmail;
        Grade = grade;
        _interests.AddRange(interests);
    }

}
