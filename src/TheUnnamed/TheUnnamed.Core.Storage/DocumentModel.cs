namespace TheUnnamed.Core.Storage;

public record DocumentModel
{
    public string Filename { get; set; } = null!;
    public Guid Uuid { get; set; }
    public string ContentType { get; set; } = null!;
}