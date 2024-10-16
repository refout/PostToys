namespace PostToys.Expression;
/// <summary> uuid表达式解析 </summary>
public class UuidExpression : IExpression
{
    /// <summary> 表达式结果计算 </summary>
    /// <param name="expression">表达式</param>
    /// <returns>表达式对应的结果</returns>
    public string Evaluate(string expression)
    {
        return expression.Replace(" ", "") switch
        {
            "$uuid" => Guid.NewGuid().ToString(),
            _ => throw new ArgumentException($"Unknown expression: {expression}.")
        };
    }

    /// <summary>
    /// 外部值传入
    /// </summary>
    /// <param name="value">传入的值</param>
    public void Input(Dictionary<string, string> value)
    {
    }
}