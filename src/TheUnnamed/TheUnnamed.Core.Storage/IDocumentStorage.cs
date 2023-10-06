namespace TheUnnamed.Core.Storage;

public interface IDocumentStorage
{
    Task<UploadResult> UploadToBucketAsync(Stream documentStream, DocumentModel document, FilemapModel filemap);
    Task<DocumentLocationDto> GetDocument(Guid uuid);
}

public record DocumentLocationDto
{
    public string ContentType { get; set; } = null!;
    public byte[] Stream { get; set; } = null!;
    public string Filename { get; set; } = null!;
}
