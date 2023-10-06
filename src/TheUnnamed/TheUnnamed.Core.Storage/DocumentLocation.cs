using Azure;
using Azure.Data.Tables;

namespace TheUnnamed.Core.Storage;

public record DocumentLocation : ITableEntity
{
    public Guid DocumentUuid { get; set; }
    public string Bucket { get; set; } = null!;
    public string Filename { get; set; } = null!;
    public string ContentType { get; set; } = null!;

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}