namespace TheUnnamed.Core.Database.Repository;

public class ReadUserEntity
{
    public Guid Uuid { get; init; } = Guid.Empty;
    public string UniqueName { get; init; } = null!;
    public string DisplayName { get; init; } = null!;
}