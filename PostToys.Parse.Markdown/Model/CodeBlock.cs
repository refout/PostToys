using PostToys.Parse.Model;

namespace PostToys.Parse.Markdown.Model;

/// <summary>
/// 代码块
/// </summary>
public class CodeBlock : Node
{
    public required string Lang { get; init; }
}