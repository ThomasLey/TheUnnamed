using TheUnnamed.Application.Shared;
using TheUnnamed.Domain.Enums;
using TheUnnamed.Domain.Primitives;

namespace TheUnnamed.Domain.Registrations;

public class TypeRegistration : ValueObject
{
    public Type Service { get; }
    public Type Implementation { get; }
    public RegistrationScope Scope { get; }

    private TypeRegistration(Type service, Type implementation, RegistrationScope scope)
    {
        Service = service;
        Implementation = implementation;
        Scope = scope;
    }

    public static Result<TypeRegistration> Create(Type service, Type implementation, RegistrationScope scope)
    {
        if (!service.IsAssignableFrom(implementation)) 
            return Result.Failure<TypeRegistration>(DomainErrors.ValueObjects.ImplementationNotImplementingService);

        return Result.Success(new TypeRegistration(service, implementation, scope));
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Service;
        yield return Implementation;
        yield return Scope;
    }
}