using TheUnnamed.Domain.Exceptions;
using TheUnnamed.Domain.Primitives;

namespace TheUnnamed.Domain;

public class Filemap : Entity
{
    private Filemap(FilemapId uuid, string title, Filemap? parent, TenantId tenant) 
        : base(uuid.Value, tenant)
    {
        Title = title;
        Parent = parent;
    }

    public Filemap? Parent { get; init; }
    public string Title { get; init; }

    public static Filemap Create(
        FilemapId uuid, string title, Filemap? parent, TenantId tenant,
        Predicate<Filemap> checkFilemapExists)
    {
        if (uuid.Value == Guid.Empty) throw new UuidCannotBeEmptyException();
        if (string.IsNullOrWhiteSpace(title)) throw new FilemapTitleCannotBeEmptyException();

        if (parent is not null && parent.Tenant != tenant) throw new TenantMismatchException("Filemap parent tenant does not match filemap tenant");
        
        var filemap = new Filemap(uuid, title, parent, tenant);
        // todo how to return an error message
        if (checkFilemapExists(filemap)) throw new FilemapAlreadyExistsException(filemap);

        return filemap;
    }

}