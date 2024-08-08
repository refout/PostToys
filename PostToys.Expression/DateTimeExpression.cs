namespace PostToys.Expression;

/// <summary> 时间表达式解析 </summary>
public class DateTimeExpression : AbstractExpression
{
    /// <summary> 获取结果 </summary>
    protected override void ToResult()
    {
        if (Properties is { Length: <= 1 or > 3 })
        {
            throw new ArgumentException($"Unknown expression: {Property}.");
        }

        var second = Properties[1].Trim();
        var dt = second.ToLower() switch
        {
            "now" => DateTime.Now,
            "today" => DateTime.Today,
            "yesterday" => DateTime.Today.AddDays(-1).Date,
            "tomorrow" => DateTime.Today.AddDays(1).Date,
            _ => throw new ArgumentException($"Unknown expression property: \"{Property}\" of \"{second}\".")
        };
        if (Properties.Length < 3)
        {
            Result = dt;
            return;
        }

        var third = Properties[2].Trim();
        Result = third.ToLower() switch
        {
            "year" => dt.Year,
            "month" => dt.Month,
            "day" => dt.Day,
            "month-day" => dt.Day,
            "year-day" => dt.DayOfYear,
            "week-day" => dt.DayOfWeek switch
            {
                DayOfWeek.Sunday => 0,
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                _ => 1
            },
            _ => throw new ArgumentException($"Unknown expression property: \"{Property}\" of \"{third}\".")
        };
    }
}