using System;
using System.Collections.Generic;
using System.Text;

namespace Domain;

public class Student
{
    public int Id { get; private set; }
    public string UserName { get; private set; }
    public string Password { get; private set; }
    public string ParentEmail { get; private set; }
    public int Grade { get; private set; }
    public Interest? Interest { get; private set; }

    protected Student() { } // For EF Core
    public Student(string userName, string password, string parentEmail, int grade, Interest? interest)
    {
        UserName = userName;
        Password = password;
        ParentEmail = parentEmail;
        Grade = grade;
        Interest = interest;
    }

}
