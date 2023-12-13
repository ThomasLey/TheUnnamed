using TheUnnamed.Domain.Exceptions;
using TheUnnamed.Domain.Primitives;

namespace TheUnnamed.Domain;

public class Tenant : Entity
{
    public string Name { get; }

    private Tenant(TenantId uuid, string name) 
        : base(uuid.Value, uuid)
    {
        Name = name;
    }

    public static Tenant Create(
        TenantId uuid, string name,
        Predicate<Tenant> checkTenantExists)
    {
        if (uuid.Value == Guid.Empty) throw new UuidCannotBeEmptyException();
        if (string.IsNullOrWhiteSpace(name)) throw new TenantNameCannotBeEmptyException();

        var tenant = new Tenant(uuid, name);
        // todo how to return an error message
        if (checkTenantExists(tenant)) throw new TenantAlreadyExistsException(tenant);

        return tenant;
    }
}