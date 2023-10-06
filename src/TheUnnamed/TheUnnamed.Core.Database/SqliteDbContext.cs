using Microsoft.EntityFrameworkCore;
using TheUnnamed.Core.Database.Config;

namespace TheUnnamed.Core.Database;

internal class SqliteDbContext : DocumentsContext
{
    private readonly string _dbPath;

    public SqliteDbContext(DatabaseConfiguration settings) : base(settings)
    {
        _dbPath = settings.ConnectionString;
        var fi = new FileInfo(_dbPath);
        if (fi.Directory == null)
            throw new DirectoryNotFoundException("The root directory for the database cannot be found");

        if (!fi.Directory.Exists)
            fi.Directory?.Create();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");

        base.OnConfiguring(options);
    }
}