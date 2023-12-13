namespace TheUnnamed.Domain.Exceptions;

public class CannotChangePermissionsException(Permission permission) : TheUnnamedExceptionBase
{
    public Permission Permission { get; } = permission ?? throw new ArgumentNullException(nameof(permission));
}