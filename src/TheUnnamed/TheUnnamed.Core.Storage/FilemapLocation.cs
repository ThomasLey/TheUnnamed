using Azure;
using Azure.Data.Tables;

namespace TheUnnamed.Core.Storage;

public record FilemapLocation : ITableEntity
{
    public Guid FilemapUuid { get; set; }
    public string Bucket { get; set; } = null!;

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}