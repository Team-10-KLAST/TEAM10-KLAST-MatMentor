namespace Application.Common.Exceptions;

public sealed class TutorLimitException : Exception
{
    public TutorLimitException(string message) : base(message) { }
}
