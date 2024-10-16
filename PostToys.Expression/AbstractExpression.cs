using System.Text.RegularExpressions;

namespace PostToys.Expression;

/// <summary> 表达式解析抽象类 </summary>
public abstract class AbstractExpression : IExpression
{
    /// <summary> 表达式 </summary>
    private string _expression = string.Empty;

    /// <summary> 表达式运算符、运算的左值及单位 </summary>
    private (char symbol, (string value, string unit) left) _operation;

    /// <summary> 表达式属性 </summary>
    protected string Property = string.Empty;

    /// <summary> 表达式的转换目标 </summary>
    private string _targetFormat = string.Empty;

    /// <summary> 表达式属性的字段 </summary>
    protected string[] Properties => Property.Split('.', StringSplitOptions.RemoveEmptyEntries);

    /// <summary> 是否需要对结果执行运算 </summary>
    private bool HasOperation => _operation.symbol != default &&
                                 _operation.left.value != string.Empty;

    /// <summary> 表达式结果计算 </summary>
    /// <param name="expression">表达式</param>
    /// <returns>表达式对应的结果</returns>
    public string Evaluate(string expression)
    {
        _expression = expression;
        // 表达式解析
        (Property, _operation, _targetFormat) = ParseExpression();

        // 获取结果
        dynamic result = ToResult();

        // 是否需要执行计算
        if (HasOperation)
        {
            result = ToBinaryOperate(result);
        }

        result = ToTarget(result);

        return result;
    }

    /// <summary> 获取结果 </summary>
    protected abstract dynamic ToResult();

    /// <summary> 将结果转换为目标格式 </summary>
    private string ToTarget(dynamic result)
    {
        return result switch
        {
            DateTime time => time.ToString(string.IsNullOrWhiteSpace(_targetFormat)
                ? "yyyy-MM-dd HH:mm:ss"
                : _targetFormat),
            int number => number.ToString(),
            _ => throw new ArgumentException(
                $"Unknown source data type that cannot be converted to the target type: {result.GetType()}.")
        };
    }

    /// <summary> 对结果执行运算 </summary>
    /// <exception cref="ArgumentException">表达式异常</exception>
    private dynamic ToBinaryOperate(dynamic result)
    {
        return result switch
        {
            DateTime dateTime => Binary(dateTime),
            int number => Binary(number),
            _ => throw new ArgumentException("The right value in a binary expression is of unknown data type.")
        };
    }

    /// <summary> 表达式解析 </summary>
    /// <returns>解析后的表达式属性</returns>
    private (string property, (char symbol, (string value, string unit) left) operation, string target)
        ParseExpression()
    {
        var property = _expression.ToLower();
        var target = string.Empty;
        (char symbol, (string value, string unit) left) operation = (default, (string.Empty, string.Empty));

        if (_expression.Contains("=>")) (property, target) = Split(_expression, "=>");

        if (property.Contains('+'))
        {
            operation.symbol = '+';
            (property, var valueUnit) = Split(property, "+");
            operation.left = ParseUnit(valueUnit);
        }
        else if (property.Contains('-'))
        {
            operation.symbol = '-';
            (property, var valueUnit) = Split(property, "-");
            operation.left = ParseUnit(valueUnit);
        }

        return (property.Replace(" ", ""), operation, target);

        (string value, string unit) ParseUnit(string value)
        {
            if (RegexGroup.NumberRegex().IsMatch(value)) return (value, string.Empty);

            var match = RegexGroup.UnitRegex().Match(value);
            return (match.Groups[1].Value.Replace(" ", ""), match.Groups[2].Value.Trim().Replace(" ", ""));
        }

        (string first, string second) Split(string input, string separator)
        {
            var res = input.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
            return res.Length switch
            {
                0 => (string.Empty, string.Empty),
                1 => (res[0].Trim(), string.Empty),
                _ => (res[0].Trim(), res[1].Trim())
            };
        }
    }

    /// <summary> 对 <see cref="DateTime" /> 进行二元计算 </summary>
    /// <param name="right">二元表达式右值</param>
    /// <returns>计算后的值</returns>
    /// <exception cref="ArgumentException">二元表达式中的时间单位参数异常</exception>
    private DateTime Binary(DateTime right)
    {
        var value = int.Parse(_operation.left.value);
        var val = _operation.symbol switch
        {
            '+' => value,
            '-' => -value,
            _ => value
        };
        return _operation.left.unit switch
        {
            "ms" => right.AddMilliseconds(val),
            "s" => right.AddSeconds(val),
            "m" => right.AddMinutes(val),
            "h" => right.AddHours(val),
            "d" => right.AddDays(val),
            "w" => right.AddDays(val * 7),
            "M" => right.AddMonths(val),
            "y" => right.AddYears(val),
            _ => throw new ArgumentException($"Unknown date-time units: {_operation.left.unit}.")
        };
    }

    /// <summary> 整形二元表达式计算 </summary>
    /// <param name="right">二元表达式右值</param>
    /// <returns>计算后的值</returns>
    /// <exception cref="ArgumentException">二元表达式中的运算符异常</exception>
    private int Binary(int right)
    {
        var left = int.Parse(_operation.left.value);
        return _operation.symbol switch
        {
            '+' => right + left,
            '-' => right - left,
            '*' => right * left,
            '/' => right / left,
            '%' => right % left,
            _ => throw new ArgumentException($"Unknown binary operator: {_operation.symbol}.")
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