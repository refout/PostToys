namespace PostToys.Parse.Markdown.Model;

/// <summary>
/// 标题
/// </summary>
public class Header : Node
{
    public required int Level { get; init; }
    public required int Order { get; init; }
}