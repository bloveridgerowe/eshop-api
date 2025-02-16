namespace Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(String message) : base(message)
    {
    }
}