using PostToys.Parse.Markdown.Model;
using PostToys.Parse.Model;

namespace PostToys.Parse.Markdown.ParseHandler;

/// <summary>
/// 代码块处理器
/// </summary>
public class ParseCodeBlock : AbstractProcessor
{
    /// <summary>
    /// 传入内容是否属于当前处理器所处理的内容
    /// </summary>
    /// <param name="content">传入内容</param>
    /// <returns>是否属于当前处理器所处理的内容</returns>
    public override bool Belong(string content)
    {
        return content.StartsWith(MarkdownFlag.CodeBlock);
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

        var lang = currentLine.Replace(MarkdownFlag.CodeBlock, "").Trim();

        currentLineIndex++;
        if (currentLineIndex >= lines.Length) return false;

        var (value, linesIndex) = ParseMultiLine(
            lines,
            ref currentLineIndex,
            Environment.NewLine,
            line => line == MarkdownFlag.CodeBlock
        );

        CodeBlock code = new()
        {
            Id = id++,
            Content = value,
            Lang = lang,
            LinesIndex = linesIndex
        };

        SetTree(nodes, node => node is Header, code);

        nodes.Add(code);

        return true;
    }
}