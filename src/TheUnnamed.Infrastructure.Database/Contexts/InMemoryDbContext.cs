using Microsoft.EntityFrameworkCore;
using TheUnnamed.Core.Database.Config;

namespace TheUnnamed.Core.Database;

internal class InMemoryDbContext : DocumentsContext
{
    public InMemoryDbContext(DatabaseConfiguration settings) : base(settings)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase(Settings!.ConnectionString);

        base.OnConfiguring(options);
    }
}