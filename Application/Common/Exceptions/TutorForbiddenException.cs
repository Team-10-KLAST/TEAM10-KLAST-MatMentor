namespace Application.Common.Exceptions;

public sealed class TutorForbiddenException : Exception
{
    public TutorForbiddenException(string message) : base(message) { }
}
