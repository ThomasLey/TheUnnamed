using TheUnnamed.Core.Database;
using TheUnnamed.Core.Database.Config;
using TheUnnamed.Application.Shared;
using TheUnnamed.Domain.Enums;
using TheUnnamed.Domain.Registrations;

namespace TheUnnamed.Infrastructure.Database
{
    // todo: document this is some kind of abstraction layer for the dependency injection. This hides the implementation and contexts.
    public static class DatabaseTypeResolver
    {
        public static Result<TypeRegistration> GetDatabaseContext(DatabaseConfiguration configuration)
        {
            return configuration.Provider.ToLower() switch
            {
                "sqlite" => TypeRegistration.Create(typeof(DocumentsContext), typeof(SqliteDbContext), RegistrationScope.Scoped),
                "mssql" => TypeRegistration.Create(typeof(DocumentsContext), typeof(MssqlDbContext), RegistrationScope.Scoped),
                "inmemory" => TypeRegistration.Create(typeof(DocumentsContext), typeof(InMemoryDbContext), RegistrationScope.Scoped),
                _ => Result.Failure<TypeRegistration>(Error.Create(
                    $"{typeof(DatabaseTypeResolver)}.NotSupported", 
                    $"The provided database '{configuration.Provider}' is not supported."))
            };
        }
    }
}
