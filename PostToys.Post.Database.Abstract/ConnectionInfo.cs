namespace PostToys.Post.Database.Abstract;

public readonly record struct ConnectionInfo
{
    public string Username{ get; init; }
    public string Password{ get; init; }
    public string Host{ get; init; }
    public string ?Port { get; init; }
    public string Database { get; init; }
}
