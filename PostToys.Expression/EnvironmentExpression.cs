using System.Text.RegularExpressions;

namespace PostToys.Expression;

/// <summary>
/// 环境变量表达式解析
/// </summary>
public class EnvironmentExpression : IExpression
{
    public Dictionary<string, string> Env { get; private set; } = [];

    /// <summary> 表达式结果计算 </summary>
    /// <param name="expression">表达式</param>
    /// <returns>表达式对应的结果</returns>
    public string Evaluate(string expression)
    {
        expression = expression.Trim();
        if (expression is { Length: < 1 })
        {
            return expression;
        }
        if (Env is { Count: <= 0 })
        {
            return expression;
        }

        if (Env.TryGetValue(expression, out var value))
        {
            return value;
        }
        return expression;
    }

    /// <summary>
    /// 外部值传入
    /// </summary>
    /// <param name="value">传入的值</param>
    public void Input(Dictionary<string, string> value)
    {
        Env = value;
    }
}