using TheUnnamed.Core.Database.Filter;

namespace TheUnnamed.Core.Database.Repository;

public interface IDocumentRepository
{
    Task<Guid> AddDocumentAsync(WriteDocumentEntity document);
    Task<IList<ReadDocumentEntity>> GetDocumentsAsync(SortAndFilter filter);
    Task<Guid> EnsureUserAsync(WriteUserEntity user);
    Task<ReadFilemapEntity> GetFilemap(Guid filemapUuid);
    bool CheckHashExists(string hash, out Guid documentUuid);
    Task<IEnumerable<ReadFilemapEntity>> GetAllFilemaps();
    Task RollbackDocumentAdd(Guid uuid);
}