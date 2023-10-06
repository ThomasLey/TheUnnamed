namespace TheUnnamed.Core.Database.Exceptions;

public class EntityNotExistException : Exception
{
    public object Entity { get; }

    public EntityNotExistException(string message, object entity) : base(message)
    {
        Entity = entity;
    }
}