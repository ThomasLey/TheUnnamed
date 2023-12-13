namespace TheUnnamed.Application.Shared
{
    public static class DomainErrors
    {
        public static readonly Error GuidCannotBeEmpty = Error.Create(nameof(DomainErrors), "A provided guid cannot be empty and should always be created through Guid.NewGuid() method.");

        public static class Document
        {
            //public static readonly Error DocumentWithHashExists = Error.Create($"{nameof(Document)}.{nameof(DocumentWithHashExists)}", "A document with the same hash already exists in the system. A document can only be added once.");
        }

        public static class User
        {
            //public static readonly Error UniqueUserNameCannotBeEmpty = Error.Create($"{nameof(User)}.{nameof(UniqueUserNameCannotBeEmpty)}", "The unique username cannot be null, empty or whitespace.");
        }

        public static class ValueObjects
        {
            public static readonly Error ImplementationNotImplementingService = Error.Create(nameof(ValueObjects), "The implementation type does not implement the registration type.");
        }
    }
}
