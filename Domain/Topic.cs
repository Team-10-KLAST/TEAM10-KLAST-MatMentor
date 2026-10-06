namespace Domain;

public class Topic
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Topic(string name)
    {
        Name = name;
    }
}
