namespace PostToys.Parse.Model;

/// <summary>
/// POST 的一个载体
/// </summary>
public record struct Toy
{
    public string Name { get; init; }
    public string Url { get; init; }
    public string Method { get; init; }
    public string Version { get; init; }
    public Dictionary<string, object> Header { get; init; }
    public Dictionary<string, object> Param { get; init; }
    public object[] PathVar { get; init; }
    public string Body { get; init; }
}