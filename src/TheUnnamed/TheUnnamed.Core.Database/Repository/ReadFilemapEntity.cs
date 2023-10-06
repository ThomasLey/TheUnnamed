namespace TheUnnamed.Core.Database.Repository;

public class ReadFilemapEntity
{
    public Guid Uuid { get; init; } = Guid.Empty;
    public Guid? ParentId { get; init; }
    public string Title { get; init; } = null!;
}