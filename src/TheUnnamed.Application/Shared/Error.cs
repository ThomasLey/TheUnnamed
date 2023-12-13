namespace TheUnnamed.Application.Shared;

public class Error
{
    private readonly string _message;
    public string Id { get; }

    private Error(string id, string message)
    {
        _message = message;
        Id = id;
    }

    public static Error Create(string id, string message) => new(id, message);
}