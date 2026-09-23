namespace Domain;

public class Interest
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    protected Interest() { }

    public Interest(string name)
    {
        Name = name;
    }

}
