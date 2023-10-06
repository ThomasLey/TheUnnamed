using System.Drawing.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using NUnit.Framework;
using TheUnnamed.Core.Database.Config;
using TheUnnamed.Core.Database.Repository;

namespace TheUnnamed.Core.Database.Tests
{
    [TestFixture]
    public class MsSqlLocalServerIntegrationTests
    {
        [Test]
        public void Test01()
        {
            var settings = new DatabaseConfiguration()
            {
                Provider = "mssql",
                ConnectionString = "Server=.,4433;Database=TheDb;User Id=sa;Password=Password01;TrustServerCertificate=true;"
            };
            var ctx = new MssqlDbContext(settings);

            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
        }

        [Test]
        [TestCase("mssql", ".,4433", typeof(MssqlDbContext))]
        [TestCase("sqlite", "foo.db", typeof(SqliteDbContext))]
        [TestCase("inmemory", "dbname-123", typeof(InMemoryDbContext))]
        public void Test02(string provider, string connectionString, Type expected)
        {
            var services = Substitute.For<IServiceCollection>();

            services.AddUnnamedDatabase(new DatabaseConfiguration()
            { Provider = provider, ConnectionString = connectionString });

            // the AddDBContext is doing some magic so it is called 4 times
            services.Received().Add(new[] {new ServiceDescriptor(
                typeof(DocumentsContext),
                expected,
                ServiceLifetime.Scoped)}.ToArray());
        }

        [Test]
        public async Task AdSomeDataWithRepo()
        {
            var ctx = new MssqlDbContext(new DatabaseConfiguration()
            {
                Provider = "mssql",
                ConnectionString =
                    "Server=.,4433;Database=TheDb;User Id=sa;Password=Password01;TrustServerCertificate=true;"
            });
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            var repo = new DocumentRepository(ctx);

            var fm = await repo.GetInboxFilemap();

            fm.Should().NotBeNull();
        }
    }
}