using Microsoft.EntityFrameworkCore;
using TheUnnamed.Core.Database.Config;

namespace TheUnnamed.Core.Database;

internal class MssqlDbContext : DocumentsContext
{
    public MssqlDbContext(DatabaseConfiguration settings) : base(settings)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Settings.ConnectionString);

        base.OnConfiguring(options);
    }
}