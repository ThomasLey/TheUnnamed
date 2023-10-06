namespace TheUnnamed.Core.Storage;

public record FilemapModel
{
    public string Title { get; set; } = null!;
    public Guid Uuid { get; set; }
}