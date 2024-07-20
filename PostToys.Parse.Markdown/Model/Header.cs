namespace PostToys.Parse.Markdown.Model;

/// <summary>
/// 标题
/// </summary>
public class Header : Node
{
    /// <summary>
    /// 标题层级
    /// </summary>
    public required int Level { get; init; }

    /// <summary>
    /// 标题序号
    /// </summary>
    public required int Order { get; init; }
}