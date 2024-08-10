using System.Text.RegularExpressions;
using PostToys.Common;

namespace PostToys.Expression;

/// <summary> 表达式解析 </summary>
public partial class ExpressionParse
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    private readonly ServiceManager _serviceManager = new();
    
    /// <summary> 构造器 </summary>
    public ExpressionParse()
    {
        _serviceManager.AddKeyedSingleton<IExpression, DateTimeExpression>("$date_time");
        _serviceManager.AddKeyedSingleton<IExpression, IdCardExpression>("$id_card");
        _serviceManager.AddKeyedSingleton<IExpression, UuidExpression>("$uuid");
        _serviceManager.AddKeyedSingleton<IExpression, EnvironmentExpression>("env");
    }

    /// <summary>
    /// 表达式正则
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"\{\{(.+?)\}\}")]
    private static partial Regex ExpressionRegex();

    /// <summary> 尝试解析出文本中的表达式 </summary>
    /// <param name="text">文本</param>
    /// <param name="expressions">表达式</param>
    /// <returns>是否成功</returns>
    public static bool TryParse(string text, out List<(string fullexpression, string expression)>? expressions)
    {
        if (string.IsNullOrEmpty(text))
        {
            expressions = [];
            return false;
        }

        var matches = ExpressionRegex().Matches(text);
        expressions = (from Match match in matches
            select (match.Value, match.Groups[1].Value)).ToList();
        return expressions.Count > 0;
    }

    /// <summary> 尝试解析出文本中的表达式，并获取其对应的值 </summary>
    /// <param name="text">文本</param>
    /// <param name="expressions">表达式及其对应的值</param>
    /// <returns>是否成功</returns>
    public bool TryParseValue(string text, out List<(string fullexpression, object result)>? expressions)
    {
        if (string.IsNullOrEmpty(text))
        {
            expressions = [];
            return false;
        }

        var matches = ExpressionRegex().Matches(text);
        expressions = (from Match match in matches
                select (match.Value, TryParseValue(match.Groups[1].Value, out object? value) ? value : match.Value))
            .ToList();
        return expressions.Count > 0;
    }

    /// <summary> 尝试通过表达式获取其对应的值 </summary>
    /// <param name="expression">表达式</param>
    /// <param name="result">表达式对应的值</param>
    /// <returns>是否成功</returns>
    public bool TryParseValue(string expression, out object? result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(expression)) return false;

        var items = expression.Split('.');
        if (items is { Length: <= 0 }) return false;

        var type = items.First().Trim().ToLower();
        if (string.IsNullOrWhiteSpace(type)) return false;

        if (!type.StartsWith('$'))
        {
            type = "env";
        }

        var parser = _serviceManager.GetKeyedService<IExpression>(type);
        if (parser == null) return false;

        try
        {
            result = parser.Evaluate(expression);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}