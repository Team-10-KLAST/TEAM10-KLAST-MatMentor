namespace Application.Common.Exceptions;

public sealed class LimitExceededException : Exception
{
    public LimitExceededException(string message) : base(message) { }
}
