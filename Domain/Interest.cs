using System;
using System.Collections.Generic;
using System.Text;

namespace Domain;

public class Interest
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Interest(string name)
    {
        Name = name;
    }

}
