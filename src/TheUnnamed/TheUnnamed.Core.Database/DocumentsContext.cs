using Microsoft.EntityFrameworkCore;
using TheUnnamed.Core.Database.Config;
using TheUnnamed.Core.Database.Entities;

namespace TheUnnamed.Core.Database;

// cannot be abstract because of migrations
public class DocumentsContext : DbContext
{
    public DocumentsContext(DatabaseConfiguration settings)
    {
        Settings = settings;
    }

    // required for database migrations
    [Obsolete]
    public DocumentsContext()
    {
    }

    protected DatabaseConfiguration? Settings { get; }

    public DbSet<User> User { get; set; } = null!;
    public DbSet<Document> Documents { get; set; } = null!;
    public DbSet<Filemap> Filemaps { get; set; } = null!;

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // we are in db migrations section, this required a database provider
        if (Settings == null)
            options.UseSqlite("Path=Documents.db");

        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasIndex(u => u.UniqueName).IsUnique();
        builder.Entity<User>().HasIndex(u => u.Uuid).IsUnique();
        builder.Entity<Document>().HasIndex(u => u.Uuid).IsUnique();
        builder.Entity<Filemap>().HasIndex(u => u.Uuid).IsUnique();

        base.OnModelCreating(builder);

        builder.Entity<Filemap>().HasData(new Filemap
        {
            // we have to set the ID here. EFCore takes care of identity insert
            // cf: https://www.learnentityframeworkcore.com/configuration/fluent-api/hasdata-method
            Id = 1,
            Uuid = Guid.NewGuid(),
            Title = "_inbox"
        });
    }
}