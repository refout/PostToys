using PostToys.Parse.Markdown.Model;
using PostToys.Parse.Model;

namespace PostToys.Parse.Markdown.ParseHandler;

/// <summary>
/// 标题处理器
/// </summary>
public class ParseHeader : AbstractProcessor
{
    /// <summary>
    /// 传入内容是否属于当前处理器所处理的内容
    /// </summary>
    /// <param name="content">传入内容</param>
    /// <returns>是否属于当前处理器所处理的内容</returns>
    public override bool Belong(string content)
    {
        return content.StartsWith(MarkdownFlag.Header);
    }

    /// <summary>
    /// 内容转为节点 <see cref="Node"/>
    /// </summary>
    /// <param name="nodes">放置节点的列表</param>
    /// <param name="lines">所有的文本行</param>
    /// <param name="currentLineIndex">当前所在行索引</param>
    /// <param name="id">id累加标识</param>
    /// <returns>是否转换成功</returns>
    public override bool TryToNode(List<Node> nodes, string[] lines, ref int currentLineIndex, ref int id)
    {
        var currentLine = lines[currentLineIndex];

        if (!Belong(currentLine)) return false;

        var level = HeaderLevel(currentLine);
        Header header = new()
        {
            Id = id++,
            Content = currentLine[level..],
            LinesIndex = [currentLineIndex + 1],
            Level = level,
            Order = HeaderOrder(nodes, level)
        };
        if (level > 1) SetTree(nodes, node => node is Header h && h.Level == level - 1, header);

        nodes.Add(header);

        return true;
    }

    private static int HeaderLevel(string content)
    {
        return string.IsNullOrWhiteSpace(content)
            ? 0
            : content.Trim().ToCharArray().TakeWhile(item => MarkdownFlag.Header == item.ToString()).Count();
    }

    private static int HeaderOrder(List<Node> nodes, int level)
    {
        var node = FindReverseNode(nodes, node => node is Header h && h.Level == level);
        return (node as Header)?.Order ?? 1;
    }
}