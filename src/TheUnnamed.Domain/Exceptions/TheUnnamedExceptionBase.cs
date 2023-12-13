namespace TheUnnamed.Domain.Exceptions;

public abstract class TheUnnamedExceptionBase : Exception
{
    protected TheUnnamedExceptionBase() { }
    protected TheUnnamedExceptionBase(string message) : base(message) { }
}