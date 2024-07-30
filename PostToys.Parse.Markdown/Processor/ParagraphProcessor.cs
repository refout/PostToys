using PostToys.Parse.Markdown.Model;
using PostToys.Parse.Model;

namespace PostToys.Parse.Markdown.Processor;

/// <summary>
/// 段落处理器
/// </summary>
public class ParagraphProcessor : AbstractProcessor
{
    /// <summary>
    /// 传入内容是否属于当前处理器所处理的内容
    /// </summary>
    /// <param name="content">传入内容</param>
    /// <returns>是否属于当前处理器所处理的内容</returns>
    public override bool Belong(string content)
    {
        return !string.IsNullOrWhiteSpace(content);
    }

    /// <summary>
    /// 内容转为节点 <see cref="Node"/>
    /// </summary>
    /// <param name="nodes">放置节点的列表</param>
    /// <param name="lines">所有的文本行</param>
    /// <param name="currentLineIndex">当前所在行索引</param>
    /// <param name="id">生成节点的id</param>
    /// <returns>是否转换成功</returns>
    public override bool TryToNode(List<Node> nodes, List<string> lines, ref int currentLineIndex, int id)
    {
        var currentLine = lines[currentLineIndex];
        if (!Belong(currentLine)) return false;

        var (value, linesIndex) = ParseMultiLine(lines, ref currentLineIndex, "", string.IsNullOrWhiteSpace);

        Paragraph paragraph = new()
        {
            Id = id,
            Content = value.Trim(),
            LinesIndex = linesIndex
        };

        SetTree(nodes, node => node is Header, paragraph);

        nodes.Add(paragraph);

        return true;
    }
}