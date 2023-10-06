namespace TheUnnamed.Core.Database.Repository;

/// <summary>
///     Entity to create documents.
/// </summary>
/// <remarks>Owner and Filemap are added as guid because "write" does only required the uuid</remarks>
public class WriteDocumentEntity
{
    public string Title { get; init; } = null!;
    public Guid FilemapUuid { get; init; } = Guid.Empty;
    public Guid OwnerUuid { get; init; } = Guid.Empty;
    public string Hash { get; init; } = null!;
    public string ContentType { get; init; } = null!;
}