namespace TheUnnamed.Domain.Primitives;

public abstract class Entity(Guid uuid, TenantId tenant) : IEquatable<Entity>
{
    #region equals & compare

    public static bool operator ==(Entity? e1, Entity e2) => e1 is not null && e1.Equals(e2);
    public static bool operator !=(Entity? e1, Entity? e2) => e1 is null || !e1.Equals(e2);

    public Guid Uuid { get; init; } = uuid;
    public TenantId Tenant { get; } = tenant;

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != GetType()) return false;
        if (obj is not Entity entity) return false;

        return Uuid.Equals(entity.Uuid) && Tenant.Equals(entity.Tenant);
    }

    public bool Equals(Entity? other)
    {
        if (other is null) return false;
        if (other.GetType() != GetType()) return false;

        return Uuid.Equals(other.Uuid) && Tenant.Equals(other.Tenant);
    }

    public override int GetHashCode()
    {
        // cf: https://stackoverflow.com/questions/3613102/why-use-a-prime-number-in-hashcode
        return Uuid.GetHashCode() * 31;
    }

    #endregion
}