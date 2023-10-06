using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using TheUnnamed.Core.Storage.Config;

[assembly: InternalsVisibleTo("TheUnnamed.Core.Storage.Tests")]
namespace TheUnnamed.Core.Storage
{

    public static class StorageStartupExtensions
    {
        public static IServiceCollection AddUnnamedStorage(this IServiceCollection services, StorageConfiguration fsConfig)
        {
            return services
                .AddUnnamedDocumentBackend(fsConfig)
                .AddRepositories();
        }

        private static IServiceCollection AddUnnamedDocumentBackend(this IServiceCollection services, StorageConfiguration configuration)
        {
            switch (configuration.Provider.ToLower())
            {
                case "filesystem":
                    throw new NotImplementedException("not implemented yet");
                    break;
                case "minio":
                    throw new NotImplementedException("not implemented yet");
                    break;
                case "azstorage":
                    services.AddScoped<IDocumentStorage, AzStorage>();
                    break;
                default:
                    throw new NotSupportedException($"The provided storage '{configuration.Provider}' is not supported.");
            }

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services;
        }

    }
}
