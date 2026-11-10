namespace Application.Common.Exceptions;

public sealed class TutorTimeoutException : Exception
{
    public TutorTimeoutException(string message) : base(message)
    {
    }
}
