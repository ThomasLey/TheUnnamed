namespace TheUnnamed.Core.Database.Exceptions;

public class EntityExistsException : Exception
{
    public object Entity { get; }

    public EntityExistsException(string message, object entity) : base(message)
    {
        Entity = entity;
    }
}