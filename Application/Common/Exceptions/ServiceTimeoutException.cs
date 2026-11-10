namespace Application.Common.Exceptions;

public sealed class ServiceTimeoutException : Exception
{
    public ServiceTimeoutException(string message) : base(message)
    {
    }
}
