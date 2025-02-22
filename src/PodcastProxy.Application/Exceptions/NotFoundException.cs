namespace PodcastProxy.Application.Exceptions;

public class NotFoundException : Exception
{
    public object? Key { get; }
    public string Name { get; }

    public NotFoundException(string name) : base(name + " was not found.")
    {
        Name = name;
    }

    public NotFoundException(string name, object key) : base($"{name} ({key}) was not found.")
    {
        Name = name;
        Key = key;
    }
}