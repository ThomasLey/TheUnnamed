namespace TheUnnamed.Domain.Exceptions;

public class TenantAlreadyExistsException(Tenant tenant) : TheUnnamedExceptionBase
{
    public Tenant Tenant { get; } = tenant ?? throw new ArgumentNullException(nameof(tenant));
}