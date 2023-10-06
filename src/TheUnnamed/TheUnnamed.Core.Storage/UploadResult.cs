namespace TheUnnamed.Core.Storage;

public record UploadResult
{
    public string Hash { get; init; } = null!;
    public string Version { get; init; } = null!;
}