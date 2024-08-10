namespace PostToys.Parse.Enum;

/// <summary>
/// 解析器类型
/// </summary>
public enum ParserType
{
    /// <summary>
    /// markdown
    /// </summary>
    Markdown
}

/// <summary>
/// 解析器类型扩展类
/// </summary>
public static class ParserTypeExtensions
{
    /// <summary>
    /// 根据解析器类型 <see cref="ParserType"/> 获取文件后缀
    /// </summary>
    /// <param name="parserType">解析器类型 <see cref="ParserType"/> </param>
    /// <returns>文件后缀</returns>
    /// <exception cref="ArgumentOutOfRangeException">未知的解析器类型</exception>
    public static string Extension(this ParserType parserType)
    {
        return parserType switch
        {
            ParserType.Markdown => "md",
            _ => throw new ArgumentOutOfRangeException(nameof(parserType), parserType, null)
        };
    }
}