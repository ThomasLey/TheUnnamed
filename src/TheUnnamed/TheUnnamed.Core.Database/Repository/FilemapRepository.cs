using TheUnnamed.Core.Database.Entities;

namespace TheUnnamed.Core.Database.Repository;

class FilemapRepository : IFilemapRepository
{
    private readonly DocumentsContext _context;

    public FilemapRepository(DocumentsContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<IEnumerable<ReadFilemapEntity>> GetAllFilemaps()
    {
        // todo null propagation does not work here. But we need the parent!
        return await Task.FromResult(_context.Filemaps.Select(fm => new ReadFilemapEntity()
        { Title = fm.Title, Uuid = fm.Uuid }).ToArray());
    }

    public async Task CreateFilemap(WriteFilemapEntity entity)
    {
        var parent = entity.ParentId.HasValue
            ? _context.Filemaps.SingleOrDefault(x => x.Uuid == entity.ParentId)
            : null;

        var f = new Filemap() { Parent = parent, Title = entity.Title, Uuid = Guid.NewGuid() };
        _context.Filemaps.Add(f);
        await _context.SaveChangesAsync();
    }
}