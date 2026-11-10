namespace Application.Common.Exceptions;

public sealed class TutorUnavailableException : Exception
{
    public TutorUnavailableException(string message) : base(message)
    {
    }
}
