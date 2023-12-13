using TheUnnamed.Domain;
using TheUnnamed.Domain.Exceptions;

namespace TheUnnamed.Domain.Exceptions;

public class UserAlreadyExistsException(User user) : TheUnnamedExceptionBase
{
    public User User { get; } = user ?? throw new ArgumentNullException(nameof(user));
}