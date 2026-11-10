namespace Application.Common.Exceptions;

public sealed class TutorConflictException : Exception
{
    public TutorConflictException(string message) : base(message) { }
}
