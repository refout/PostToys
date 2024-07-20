namespace PostToys.Parse.Markdown.Model;

/// <summary>
/// 代码块
/// </summary>
public class CodeBlock : Node
{
    /// <summary>
    /// 代码语言
    /// </summary>
    public required string Lang { get; init; }
}