using System;
using System.Text.RegularExpressions;

namespace PostToys.Expression;

/// <summary>
/// 正则表达式
/// </summary>
public partial class RegexGroup
{
    /// <summary>
    /// 表达式正则
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\{\{(.+?)\}\}")]
    public static partial Regex ExpressionRegex();

    /// <summary> 匹配时间数值及单位的正则表达式 </summary>
    /// <returns>正则表达式</returns>
    [GeneratedRegex(@"\b(\d+)([Mmsdyhw]|ms)\b")]
    public static partial Regex UnitRegex();

    /// <summary> 匹配数字的正则表达式 </summary>
    /// <returns>正则表达式</returns>
    [GeneratedRegex(@"\b(\d+)\b")]
    public static partial Regex NumberRegex();
}
