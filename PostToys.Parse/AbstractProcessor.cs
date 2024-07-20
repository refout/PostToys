using System.Text;
using PostToys.Parse.Model;

namespace PostToys.Parse;

/// <summary>
/// 处理器抽象类
/// </summary>
public abstract class AbstractProcessor
{
    /// <summary>
    /// 传入内容是否属于当前处理器所处理的内容
    /// </summary>
    /// <param name="content">传入内容</param>
    /// <returns>是否属于当前处理器所处理的内容</returns>
    public abstract bool Belong(string content);

    /// <summary>
    /// 内容转为节点 <see cref="Node"/>
    /// </summary>
    /// <param name="nodes">放置节点的列表</param>
    /// <param name="lines">所有的文本行</param>
    /// <param name="currentLineIndex">当前所在行索引</param>
    /// <param name="id">生成节点的id</param>
    /// <returns>是否转换成功</returns>
    public abstract bool TryToNode(List<Node> nodes, string[] lines, ref int currentLineIndex, int id);

    /// <summary>
    /// 解析多行内容为一个节点
    /// </summary>
    /// <param name="lines">所有的文本行</param>
    /// <param name="currentLineIndex">当前所在行索引</param>
    /// <param name="separator">多行内容转换为节点内容的分隔符</param>
    /// <param name="handlerLineFunc">每行内容的处理方法</param>
    /// <param name="finishFunc">读取多行内容结束的标识</param>
    /// <returns>content: 最终获取到的节点的内容， linesIndex：多行内容的行索引</returns>
    protected static (string content, List<int> linesIndex) ParseMultiLine(
        string[] lines, ref int currentLineIndex, string separator,
        Func<string, string> handlerLineFunc, Func<string, bool> finishFunc)
    {
        StringBuilder builder = new();

        List<int> linesIndex = [];

        var currentLine = lines[currentLineIndex];

        while (!finishFunc.Invoke(currentLine))
        {
            builder.Append(handlerLineFunc.Invoke(currentLine)).Append(separator);

            linesIndex.Add(currentLineIndex + 1);

            if (currentLineIndex + 1 >= lines.Length) break;
            currentLine = lines[++currentLineIndex];
        }

        builder.Remove(builder.Length - separator.Length, separator.Length);
        return (builder.ToString(), linesIndex);
    }

    /// <summary>
    /// 解析多行内容为一个节点
    /// </summary>
    /// <param name="lines">所有的文本行</param>
    /// <param name="currentLineIndex">当前所在行索引</param>
    /// <param name="separator">多行内容转换为节点内容的分隔符</param>
    /// <param name="finishFunc">读取多行内容结束的标识</param>
    /// <returns>content: 最终获取到的节点的内容， linesIndex：多行内容的行索引</returns>
    protected static (string, List<int> linesIndex) ParseMultiLine(string[] lines, ref int currentLineIndex,
        string separator, Func<string, bool> finishFunc)
    {
        return ParseMultiLine(lines, ref currentLineIndex, separator, currentLine => currentLine, finishFunc);
    }

    /// <summary>
    /// 设置节点的树结构信心，包括父id，子节点id
    /// </summary>
    /// <param name="nodes">所有节点列表</param>
    /// <param name="matchFunc">匹配到的标识</param>
    /// <param name="current">当前节点</param>
    protected static void SetTree(List<Node> nodes, Func<Node, bool> matchFunc, Node current)
    {
        var parent = FindReverseNode(nodes, matchFunc);
        if (parent == null) return;

        current.ParentId = parent.Id;
        parent.ChildrenId ??= [];
        parent.ChildrenId.Add(current.Id);
    }

    /// <summary>
    /// 倒叙查找符合要求的节点
    /// </summary>
    /// <param name="nodes">所有节点列表</param>
    /// <param name="matchFunc">查找要求</param>
    /// <returns>查找到的节点</returns>
    protected static Node? FindReverseNode(List<Node> nodes, Func<Node, bool> matchFunc)
    {
        var length = nodes.Count;
        for (var i = length - 1; i >= 0; i--)
        {
            var node = nodes[i];
            if (matchFunc.Invoke(node)) return node;
        }

        return null;
    }
}