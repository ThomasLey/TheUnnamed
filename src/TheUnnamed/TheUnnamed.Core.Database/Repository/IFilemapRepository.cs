namespace TheUnnamed.Core.Database.Repository;

public interface IFilemapRepository
{
    Task<IEnumerable<ReadFilemapEntity>> GetAllFilemaps();

    Task CreateFilemap(WriteFilemapEntity entity);
}