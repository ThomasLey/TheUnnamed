using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using TheUnnamed.Core.Database.Config;
using TheUnnamed.Core.Database.Repository;

[assembly: InternalsVisibleTo("TheUnnamed.Core.Database.Tests")]
namespace TheUnnamed.Core.Database
{

    public static class DatabaseStartupExtensions
    {
        public static IServiceCollection AddUnnamedDatabase(this IServiceCollection services, DatabaseConfiguration dbConfig)
        {
            return services
                .AddUnnamedDbContext(dbConfig)
                .AddRepositories();
        }

        private static IServiceCollection AddUnnamedDbContext(this IServiceCollection services, DatabaseConfiguration configuration)
        {
            switch (configuration.Provider.ToLower())
            {
                case "sqlite":
                    services.AddDbContext<DocumentsContext, SqliteDbContext>();
                    break;
                case "mssql":
                    services.AddDbContext<DocumentsContext, MssqlDbContext>();
                    break;
                case "inmemory":
                    services.AddDbContext<DocumentsContext, InMemoryDbContext>();
                    break;
                default:
                    throw new NotSupportedException($"The provided database '{configuration.Provider}' is not supported.");
            }

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IFilemapRepository, FilemapRepository>();

            return services;
        }

    }
}
