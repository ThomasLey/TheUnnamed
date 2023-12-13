namespace TheUnnamed.Domain.Exceptions;

public class DocumentAlreadyExistsException(Document document) : TheUnnamedExceptionBase
{
    public Document Document { get; } = document ?? throw new ArgumentNullException(nameof(document));
}