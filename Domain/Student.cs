namespace Domain;

public class Student
{

    private readonly List<Interest> _interests = new();
    public int Id { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string ParentEmail { get; private set; }
    public int Grade { get; private set; }
    public IReadOnlyCollection<Interest> Interests => _interests.AsReadOnly();

    protected Student() { } // For EF Core
    public Student(string userName, string password, string parentEmail, int grade, IEnumerable<Interest> interests)
    {
        UserName = userName;
        Password = password;
        ParentEmail = parentEmail;
        Grade = grade;
        _interests = interests.ToList();
    }

}
