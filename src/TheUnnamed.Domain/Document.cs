using TheUnnamed.Domain.Exceptions;
using TheUnnamed.Domain.Primitives;

namespace TheUnnamed.Domain;

public class Document : Entity
{
    private Document(
        DocumentId uuid, string title, Filemap filemap, ContentTypeInfo info, HashAndSize hash,
        TenantId tenantId) : base(uuid.Value, tenantId)
    {
        Title = title;
        Filemap = filemap;
        Info = info;
        Hash = hash;
        Permissions = new List<Permission>();
    }

    public string Title { get; init; }
    public Filemap Filemap { get; init; }
    public ContentTypeInfo Info { get; init; }
    public HashAndSize Hash { get; init; }
    public IEnumerable<Permission> Permissions { get; init; }

    public Permission AddPermission(User user, PermissionLevel level)
    {
        var result = Permission.Create(new PermissionId(Guid.NewGuid()), this, user, level, Tenant, CheckPermissions);
        ((IList<Permission>)Permissions).Add(result);

        // this is an awesome inline function
        bool CheckPermissions(Permission p)
        {
            if (Permissions.All(x => x.User != p.User)) return true;

            // todo
            return true;
        }

        // todo remove old permissions

        return result;
    }

    public static Document Create(
        DocumentId uuid, string title, Filemap filemap, ContentTypeInfo info, HashAndSize hash, User user,
        TenantId tenantId,
        Predicate<Document> checkDocumentExists)
    {
        if (uuid.Value == Guid.Empty) throw new UuidCannotBeEmptyException();
        if (filemap.Tenant != tenantId) throw new TenantMismatchException("Filemap tenant does not match document tenant.");

        var document = new Document(uuid, title, filemap, info, hash, tenantId);
        // todo how to return an error message
        if (checkDocumentExists(document)) throw new DocumentAlreadyExistsException(document);

        document.AddPermission(user, PermissionLevel.Owner);
        return document;
    }
}