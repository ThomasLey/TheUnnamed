namespace TheUnnamed.Domain.Exceptions;

public class FilemapAlreadyExistsException(Filemap filemap) : TheUnnamedExceptionBase
{
    public Filemap Filemap { get; } = filemap ?? throw new ArgumentNullException(nameof(filemap));
}