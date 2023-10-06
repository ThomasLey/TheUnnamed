namespace TheUnnamed.Core.Database.Repository;

public interface IFilemapRepository
{
    Task<IEnumerable<ReadFilemapEntity>> GetAllFilemaps();

    Task<ReadFilemapEntity> CreateFilemap(WriteFilemapEntity entity);
}