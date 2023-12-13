using TheUnnamed.Domain.Exceptions;
using TheUnnamed.Domain.Primitives;

namespace TheUnnamed.Domain
{
    public class User : Entity
    {
        private User(UserId uuid, string uniqueName, string displayName, TenantId tenant) 
            : base(uuid.Value, tenant)
        {
            UniqueName = uniqueName;
            DisplayName = displayName;
        }

        public string UniqueName { get; init; }
        public string DisplayName { get; init; }

        public static User Create(
            UserId uuid, string uniqueName, string displayName,
            TenantId tenant,
            Predicate<User> checkUserExists)
        {
            if (uuid.Value == Guid.Empty) throw new UuidCannotBeEmptyException();
            if (string.IsNullOrWhiteSpace(uniqueName)) throw new UserUniqueNameCannotBeEmptyException();

            var displayNameSafe = displayName;
            if (string.IsNullOrWhiteSpace(displayNameSafe)) displayNameSafe = uniqueName;

            var user = new User(uuid, uniqueName, displayNameSafe, tenant);
            // todo how to return an error message
            if (checkUserExists(user)) throw new UserAlreadyExistsException(user);

            return user;
        }
    }
}