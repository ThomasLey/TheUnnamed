namespace TheUnnamed.Core.Database.Repository;

public class ReadDocumentEntity
{
    public Guid Uuid { get; init; } = Guid.Empty;
    public string Title { get; init; } = null!;
    public ReadFilemapEntity Filemap { get; init; } = null!;
    public ReadUserEntity Owner { get; init; } = null!;
    public string Hash { get; init; } = null!;
    public string ContentType { get; init; } = null!;

    public ReadUserEntity ModifiedBy { get; init; } = null!;
    public DateTime ModifiedAt { get; init; } = DateTime.MinValue;
}