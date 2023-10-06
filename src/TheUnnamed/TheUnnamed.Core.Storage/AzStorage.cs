using System.Reflection;
using Azure.Storage.Blobs;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Azure.Data.Tables;
using TheUnnamed.Core.Storage.Config;
using System.Net.Sockets;
using System.ComponentModel;
using System.IO;

namespace TheUnnamed.Core.Storage
{
    [Guid("99A555E1-0C40-452E-8C20-9D89D4D4C4CF")]
    public class AzStorage : IDocumentStorage
    {
        private readonly StorageConfiguration _config;

        public AzStorage(StorageConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<UploadResult> UploadToBucketAsync(Stream documentStream, DocumentModel document, FilemapModel filemap)
        {
            // this connection string works smoothly
            var blobServiceClient = new BlobServiceClient(_config.ConnectionString);
            var tableServiceClient = new TableServiceClient(_config.ConnectionString);

            var tableClient = tableServiceClient.GetTableClient(tableName: "Inventory");
            await tableClient.CreateIfNotExistsAsync();
            var blobContainerClient = await EnsureBucket(filemap, blobServiceClient, tableServiceClient);

            // hack: this can only add new documents!
            var res = await blobContainerClient.UploadBlobAsync(document.Filename, documentStream);

            var result = new UploadResult()
            {
                Hash = string.Empty,
                Version = res.Value.VersionId
            };

            var docLoc = new DocumentLocation()
            {
                Bucket = blobContainerClient.Name,
                Filename = document.Filename,
                DocumentUuid = document.Uuid,
                ContentType = document.ContentType,
                PartitionKey = filemap.Uuid.ToString(),
                RowKey = document.Uuid.ToString(),
                Timestamp = DateTimeOffset.UtcNow
            };
            await tableClient.AddEntityAsync(docLoc);

            return result;
        }

        public async Task<DocumentLocationDto> GetDocument(Guid uuid)
        {
            // this connection string works smoothly
            var blobServiceClient = new BlobServiceClient(_config.ConnectionString);
            var tableServiceClient = new TableServiceClient(_config.ConnectionString);

            var tableClient = tableServiceClient.GetTableClient(tableName: "Inventory");
            await tableClient.CreateIfNotExistsAsync();
            var document = tableClient.Query<DocumentLocation>(x => x.DocumentUuid == uuid).FirstOrDefault();
            if (document == null)
                return null!;

            var blobContainerClient = blobServiceClient.GetBlobContainerClient(document.Bucket);
            var blobClient = blobContainerClient.GetBlobClient(document.Filename);
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Cannot find file {document.Filename} in bucket {document.Bucket}");
            }

            var stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);
            var contents = stream.ToArray();

            return new DocumentLocationDto()
            {
                ContentType = document.ContentType,
                Stream = contents,
                Filename = document.Filename
            };
        }

        /*****  Misc. Helper Methods **********/
        private async Task<BlobContainerClient> EnsureBucket(
            FilemapModel filemap,
            BlobServiceClient blobServiceClient,
            TableServiceClient tableServiceClient)
        {
            var tableClient = tableServiceClient.GetTableClient(tableName: "FilemapBuckets");
            await tableClient.CreateIfNotExistsAsync();

            // lets try to get the bucket from azure table
            var storedFilemapLoc = tableClient.GetEntityIfExists<FilemapLocation>(filemap.Uuid.ToString()[..4], filemap.Uuid.ToString());
            var bucket = storedFilemapLoc.HasValue ? storedFilemapLoc.Value.Bucket : CreateBucketFromFilemap(filemap);

            // in case filemap location is not stored in table, do it now
            if (!storedFilemapLoc.HasValue)
            {
                var filemapLocation = new FilemapLocation()
                {
                    FilemapUuid = filemap.Uuid,
                    Bucket = bucket,

                    PartitionKey = filemap.Uuid.ToString()[..4],
                    RowKey = filemap.Uuid.ToString(),
                    Timestamp = DateTimeOffset.UtcNow
                };
                await tableClient.AddEntityAsync(filemapLocation);
            }

            // lets try to get an existing container
            var result = blobServiceClient.GetBlobContainerClient(bucket)
                         ?? (await blobServiceClient.CreateBlobContainerAsync(bucket)).Value;

            await result.CreateIfNotExistsAsync();

            return result;
        }

        private string CreateBucketFromFilemap(FilemapModel filemap)
        {
            var cleanName = Regex.Replace(filemap.Title, "[^A-Za-z0-9]", "");
            var bucketName = cleanName + "-" + filemap.Uuid.ToString().Replace("-", "").ToLower();
            return bucketName.ToLower();
        }
    }
}