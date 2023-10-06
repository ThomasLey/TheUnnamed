using TheUnnamed.Core.Database.Entities;
using TheUnnamed.Core.Database.Exceptions;
using TheUnnamed.Core.Database.Filter;

namespace TheUnnamed.Core.Database.Repository;

internal class DocumentRepository : IDocumentRepository
{
    private readonly DocumentsContext _context;

    public DocumentRepository(DocumentsContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddDocumentAsync(WriteDocumentEntity document)
    {
        // check for foreign keys and look up their real identities
        var user = _context.User.SingleOrDefault(x => x.Uuid == document.OwnerUuid)
                   ?? throw new EntityNotExistException($"The provided owner with uuid {document.OwnerUuid} does not exist", document.OwnerUuid);
        var filemap = _context.Filemaps.SingleOrDefault(x => x.Uuid == document.FilemapUuid)
                      ?? throw new EntityNotExistException($"The provided owner with uuid {document.FilemapUuid} does not exist", document.FilemapUuid);

        // todo add some awesome mapping framework
        var entity = new Document
        {
            Uuid = Guid.NewGuid(),
            Title = document.Title,
            Filemap = filemap,
            Owner = user,
            Hash = document.Hash,
            ChangeInfo = new ChangeInfo
            {
                ModifiedBy = user,
                ModifiedAt = DateTime.UtcNow
            },
            ContentType = document.ContentType
        };

        await _context.Documents.AddAsync(entity).ConfigureAwait(true);
        await _context.SaveChangesAsync().ConfigureAwait(true);

        return entity.Uuid;
    }

    public async Task<IList<ReadDocumentEntity>> GetDocumentsAsync(SortAndFilter filter)
    {
        return _context.Documents
            .ToFilterView(filter)
            .Select(x => new ReadDocumentEntity
            {
                Uuid = x.Uuid,
                Title = x.Title,
                // filemap with lookup? Filemap = new FilemapEntity() { Uuid = x.Filemap.Uuid, ParentId = x.Filemap.ParentId, Title = x.Filemap.Title },
                Filemap = new ReadFilemapEntity() { Uuid = x.Filemap.Uuid, Title = x.Filemap.Title },
                Owner = new ReadUserEntity
                {
                    Uuid = x.Owner.Uuid,
                    UniqueName = x.Owner.UniqueName,
                    DisplayName = x.Owner.DisplayName
                },
                Hash = x.Hash,
                ModifiedBy = new ReadUserEntity()
                {
                    Uuid = x.ChangeInfo.ModifiedBy.Uuid,
                    UniqueName = x.ChangeInfo.ModifiedBy.UniqueName,
                    DisplayName = x.ChangeInfo.ModifiedBy.DisplayName
                },
                ModifiedAt = x.ChangeInfo.ModifiedAt,
                ContentType = x.ContentType
            })
            .ToList(); ;
    }

    public async Task<ReadFilemapEntity> GetInboxFilemap()
    {
        // that's shit, every storage need to take care of the file location for a filemap
        var fm = _context.Filemaps.SingleOrDefault(x => x.Title == "_inbox");
        if (fm != null)
            return new ReadFilemapEntity() { Title = fm.Title, Uuid = fm.Uuid, ParentId = fm.Parent?.Uuid };

        var newFm = new Filemap()
        {
            Title = "Inbox",
            Uuid = Guid.NewGuid()
        };
        _context.Filemaps.Add(newFm);
        await _context.SaveChangesAsync();
        return new ReadFilemapEntity() { Title = newFm.Title, Uuid = newFm.Uuid };
    }

    public async Task<ReadFilemapEntity> GetFilemap(Guid filemapUuid)
    {
        // that's shit, every storage need to take care of the file location for a filemap
        var fm = _context.Filemaps.SingleOrDefault(x => x.Uuid == filemapUuid);
        if (fm != null)
            return new ReadFilemapEntity() { Title = fm.Title, Uuid = fm.Uuid, ParentId = fm.Parent?.Uuid };

        throw new KeyNotFoundException();
    }

    /// <summary>
    /// Check if a document with the uuid already exists
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="documentUuid"></param>
    /// <returns>True if a document can be found with the given hash</returns>
    public bool CheckHashExists(string hash, out Guid documentUuid)
    {
        var document = _context.Documents.SingleOrDefault(x => x.Hash == hash);
        documentUuid = document?.Uuid ?? Guid.Empty;
        return document != null;
    }

    public async Task<IEnumerable<ReadFilemapEntity>> GetAllFilemaps()
    {
        // todo null propagation does not work here. But we need the parent!
        return await Task.FromResult(_context.Filemaps.Select(fm => new ReadFilemapEntity()
        { Title = fm.Title, Uuid = fm.Uuid }).ToArray());
    }

    public async Task RollbackDocumentAdd(Guid uuid)
    {
        var document = _context.Documents.Single(x => x.Uuid == uuid);
        _context.Documents.Remove(document);
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> EnsureUserAsync(WriteUserEntity user)
    {
        var usr = _context.User.SingleOrDefault(x => x.UniqueName == user.UniqueName);
        if (usr != null)
            return usr.Uuid;

        var newUsr = new User()
        {
            DisplayName = user.DisplayName,
            UniqueName = user.UniqueName,
            Uuid = Guid.NewGuid()
        };
        _context.User.Add(newUsr);
        await _context.SaveChangesAsync();
        return newUsr.Uuid;
    }
}