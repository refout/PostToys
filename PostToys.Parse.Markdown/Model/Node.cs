namespace PostToys.Parse.Markdown.Model;

/// <summary>
/// 文档节点
/// </summary>
public abstract class Node
{
    /// <summary>
    /// id，唯一标识
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// 节点内容
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// 节点父id
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 节点下的子节点id集合
    /// </summary>
    public List<int>? ChildrenId { get; set; }

    /// <summary>
    /// 节点对应文档的行索引
    /// </summary>
    public required List<int> LinesIndex { get; init; }
}