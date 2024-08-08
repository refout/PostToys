namespace PostToys.Expression;

/// <summary>
/// 表达式接口
/// </summary>
public interface IExpression
{
    /// <summary>
    /// 计算表达式结果
    /// </summary>
    /// <param name="expression">表达式，E.g：<code>$date_time.now</code></param>
    /// <returns>表达式所代表的值</returns>
    dynamic Evaluate(string expression);
}