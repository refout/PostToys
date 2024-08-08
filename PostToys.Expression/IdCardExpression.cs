using PostToys.Variable;

namespace PostToys.Expression;

/// <summary> 证件号表达式解析 </summary>
public class IdCardExpression : IExpression
{
    /// <summary> 表达式结果计算 </summary>
    /// <param name="expression">表达式</param>
    /// <returns>表达式对应的结果</returns>
    public dynamic Evaluate(string expression)
    {
        return expression.Replace(" ", "") switch
        {
            "$id_card" => RandomIdCard.Number,
            _ => throw new ArgumentException($"Unknown expression: {expression}.")
        };
    }
}