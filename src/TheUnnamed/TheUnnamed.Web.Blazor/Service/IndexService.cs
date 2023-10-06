using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using TheUnnamed.Core.Database.Exceptions;
using TheUnnamed.Core.Database.Repository;
using TheUnnamed.Core.Storage;
using Index = TheUnnamed.Web.Blazor.Pages.Index;

namespace TheUnnamed.Web.Blazor.Service;

public class FilemapOverviewService
{
    private readonly ILogger<IndexService> _logger;
    private readonly IFilemapRepository _database;

    public FilemapOverviewService(ILogger<IndexService> logger, IFilemapRepository database, AuthenticationStateProvider authentication)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }
    public async Task<Dictionary<Guid, string>> GetFilemaps()
    {
        return (await _database.GetAllFilemaps()).ToDictionary(x => x.Uuid, x => x.Title);
    }
}

public class IndexService
{
    private readonly ILogger<IndexService> _logger;
    private readonly IDocumentStorage _storage;
    private readonly IDocumentRepository _database;
    private readonly AuthenticationStateProvider _authentication;
    private const long MaxFileSize = 1024 * 1024 * 30; // represents 30 MB

    public IndexService(ILogger<IndexService> logger, IDocumentStorage storage, IDocumentRepository database, AuthenticationStateProvider authentication)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
    }

    public async Task ExecuteUploadAsync(IBrowserFile file, Index.CreateDocument documentInfo)
    {
        _logger.LogDebug($"Uploading '{file.Name}' to bucket '{documentInfo.Filemap}'");

        // here I need to validate the hash!
        await using var hashStream = file.OpenReadStream(MaxFileSize);
        var hash = await GetHashAsync<SHA256CryptoServiceProvider>(hashStream);
        var hashExists = _database.CheckHashExists(hash, out Guid documentUuid);
        if (hashExists)
        {
            _logger.LogInformation($"The document {documentUuid} is already uploaded with the hash {hash}");
            throw new EntityExistsException("The document with this hash already exists", documentUuid);
        }

        var userUuid = await _database.EnsureUserAsync(new WriteUserEntity()
        {
            DisplayName = _authentication.GetAuthenticationStateAsync().Result.User.Identity?.Name,
            UniqueName = _authentication.GetAuthenticationStateAsync().Result.User.Identity?.Name
        });

        // now we need to do the database shit
        var documentId = await _database.AddDocumentAsync(new WriteDocumentEntity()
        {
            ContentType = file.ContentType,
            FilemapUuid = documentInfo.Filemap,
            Hash = hash,
            OwnerUuid = userUuid,
            Title = file.Name
        });

        // lets do the document shit
        var filemap = await _database.GetFilemap(documentInfo.Filemap);

        try
        {
            //wait using var uploadStream = file.OpenReadStream(MaxFileSize);
            await _storage.UploadToBucketAsync(
                hashStream, 
                new DocumentModel() { Filename = file.Name, Uuid = documentId, ContentType = file.ContentType}, 
                new FilemapModel() { Title = filemap.Title, Uuid = filemap.Uuid });
            _logger.LogDebug($"Document {documentInfo.Filename} uploaded successfully.");
        }
        catch (Exception e)
        {
            _logger.LogInformation("cannot upload document. rollbck transaciton");
            await _database.RollbackDocumentAdd(documentId);
            throw new OperationCanceledException("The operation was canceled due to an error", e);
        }

        _logger.LogInformation($"Document '{file.Name}' was uploaded to the bucket '{documentInfo.Filemap}'");
    }

    public async Task<Dictionary<Guid, string>> GetFilemaps()
    {
        return (await _database.GetAllFilemaps()).ToDictionary(x => x.Uuid, x => x.Title);
    }

    public static async Task<string> GetHashAsync<T>(Stream stream)
        where T : HashAlgorithm, new()
    {
        StringBuilder sb;

        using (var algo = new T())
        {
            var buffer = new byte[8192];
            int bytesRead;

            // compute the hash on 8KiB blocks
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                algo.TransformBlock(buffer, 0, bytesRead, buffer, 0);
            algo.TransformFinalBlock(buffer, 0, bytesRead);

            // build the hash string
            sb = new StringBuilder(algo.HashSize / 4);
            foreach (var b in algo.Hash)
                sb.AppendFormat("{0:x2}", b);
        }

        return sb?.ToString();
    }
}