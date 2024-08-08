namespace PostToys.Expression;

/// <summary>
/// 环境变量表达式解析
/// </summary>
public class EnvironmentExpression : IExpression
{
    /// <summary> 表达式结果计算 </summary>
    /// <param name="expression">表达式</param>
    /// <returns>表达式对应的结果</returns>
    public dynamic Evaluate(string expression)
    {
        throw new NotImplementedException();
    }
}