using TheUnnamed.Domain.Exceptions;
using TheUnnamed.Domain.Primitives;

namespace TheUnnamed.Domain;

public class Permission : Entity
{
    public Document Document { get; }
    public User User { get; }
    public PermissionLevel PermissionLevel { get; }

    private Permission(
        PermissionId uuid, Document document, User user, PermissionLevel permissionLevel, TenantId tenant) 
        : base(uuid.Value, tenant)
    {
        Document = document;
        User = user;
        PermissionLevel = permissionLevel;
    }

    public static Permission Create(
        PermissionId uuid, Document document, User user, PermissionLevel permissionLevel, 
        TenantId tenant,
        Predicate<Permission> canChangePermissions)
    {
        if (uuid.Value == Guid.Empty) throw new UuidCannotBeEmptyException();
        if (document.Tenant != tenant) throw new TenantMismatchException("Document tenant does not match document tenant.");
        if (user.Tenant != tenant) throw new TenantMismatchException("User tenant does not match document tenant.");

        var permission = new Permission(uuid, document, user, permissionLevel, tenant);
        // todo how to return an error message
        if (canChangePermissions(permission)) throw new CannotChangePermissionsException(permission);

        return permission;
    }
}