using System.Diagnostics.CodeAnalysis;

namespace PostToys.Variable;

/// <summary>
/// 生成随机时间
/// </summary>
public static class RandomDateTime
{
    /// <summary>
    /// 1970-01-01 00:00:00
    /// </summary>
    private static readonly DateTime DateTime1997 = new(1970, 1, 1, 0, 0, 0);

    /// <summary>
    /// 获取当前时间
    /// </summary>
    /// <param name="addYears">当前时间+输入年</param>
    /// <param name="addMonths">当前时间+输入月</param>
    /// <param name="addDays">当前时间+输入天</param>
    /// <param name="addHours">当前时间+输入小时</param>
    /// <param name="addMinutes">当前时间+输入分</param>
    /// <param name="addSeconds">当前时间+输入秒</param>
    /// <param name="addMilliseconds">当前时间+输入毫秒</param>
    /// <param name="addMicroseconds">当前时间+输入微妙</param>
    /// <param name="format">时间格式</param>
    /// <returns>当前时间+传入时间的日期时间</returns>
    public static string Now(
        int addYears = 0,
        int addMonths = 0,
        double addDays = 0,
        double addHours = 0,
        double addMinutes = 0,
        double addSeconds = 0,
        double addMilliseconds = 0,
        double addMicroseconds = 0,
        [StringSyntax("DateTimeFormat")] string format = "yyyy-MM-dd HH:mm:ss.fff") =>
        DateTime.Now
            .AddYears(addYears)
            .AddMonths(addMonths)
            .AddDays(addDays)
            .AddHours(addHours)
            .AddMinutes(addMinutes)
            .AddSeconds(addSeconds)
            .AddMilliseconds(addMilliseconds)
            .AddMicroseconds(addMicroseconds).ToString(format);

    /// <summary>
    /// 获取给定范围的随机时间
    /// </summary>
    /// <param name="startDate">开始时间</param>
    /// <param name="endDate">结束时间</param>
    /// <param name="format">时间格式</param>
    /// <returns>给定范围的随机时间</returns>
    public static string RangeFormat(
        DateTime startDate, DateTime endDate,
        [StringSyntax("DateTimeFormat")] string format = "yyyy-MM-dd HH:mm:ss.fff"
    ) => Range(startDate, endDate).ToString(format);

    public static DateTime Range(DateTime startDate, DateTime endDate)
    {
        var startMillisecond = Convert.ToInt64((startDate - DateTime1997).TotalMilliseconds);
        var endMillisecond = Convert.ToInt64((endDate - DateTime1997).TotalMilliseconds);
        if (startMillisecond > endMillisecond)
        {
            (startMillisecond, endMillisecond) = (endMillisecond, startMillisecond);
        }

        var randomMillisecond = RandomNumber.Long(startMillisecond, endMillisecond);

        return new DateTime(DateTime1997.Ticks + randomMillisecond * 10000);
    }
}