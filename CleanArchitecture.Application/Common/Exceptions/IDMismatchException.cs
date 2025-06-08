namespace CleanArchitecture.Application.Common.Exceptions;

public class IDMismatchException : Exception
{
    public IDMismatchException(string? message) : base(message)
    {
    }
}
