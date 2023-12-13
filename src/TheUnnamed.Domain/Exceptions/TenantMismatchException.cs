namespace TheUnnamed.Domain.Exceptions;

public class TenantMismatchException : TheUnnamedExceptionBase
{
    public TenantMismatchException(string message) :base(message) { }
}