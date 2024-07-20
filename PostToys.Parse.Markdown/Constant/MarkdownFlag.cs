using System.ComponentModel;
using System.Reflection;

namespace PostToys.Parse.Markdown.Constant;

/// <summary>
/// markdown 文档节点的标识
/// </summary>
public static class MarkdownFlag
{
    /// <summary>
    /// 标题
    /// </summary>
    [Flag] public const string Header = "#";

    /// <summary>
    /// 引用块
    /// </summary>
    [Flag] public const string Blockquote = ">";

    /// <summary>
    /// 代码块
    /// </summary>
    [Flag] public const string CodeBlock = "```";

    /// <summary>
    /// 段落
    /// </summary>
    [Description] [Flag(Default = true)] public const string Paragraph = "";

    /// <summary>
    /// markdown 文档节点的标识列表，不包含段落 <see cref="Paragraph"/>
    /// </summary>
    private static readonly List<string> SpecificFlags = typeof(MarkdownFlag).GetFields()
        .Where(f => Attribute.IsDefined(f, typeof(FlagAttribute)))
        .Where(f => !f.GetCustomAttribute<FlagAttribute>(true)!.Default)
        .Select(f => f.GetValue(null))
        .Cast<string>()
        .ToList();

    /// <summary>
    /// 添加标识
    /// </summary>
    /// <param name="flag"></param>
    public static void AddFlag(string flag)
    {
        if (SpecificFlags.Contains(flag)) return;

        SpecificFlags.Add(flag);
    }

    /// <summary>
    /// 是否包含传入标识
    /// </summary>
    /// <param name="flag">传入标识</param>
    /// <returns>是否包含传入标识</returns>
    public static bool Contains(string flag)
    {
        return SpecificFlags.Contains(flag);
    }

    /// <summary>
    /// 传入文本内容转换为 markdown 标识
    /// </summary>
    /// <param name="content">文本内容</param>
    /// <returns>markdown 标识，未匹配到，默认返回 <see cref="Paragraph"/></returns>
    public static string ToFlag(string content)
    {
        foreach (var flag in SpecificFlags.Where(content.StartsWith))
        {
            return flag;
        }

        return Paragraph;
    }
}

/// <summary>
/// 用于标识 markdown 标识字段的特性
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class FlagAttribute : Attribute
{
    /// <summary>
    /// 是否为默认标识
    /// </summary>
    public bool Default { get; set; }
}