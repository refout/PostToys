namespace PostToys.Variable;

/// <summary>
/// 随机数字
/// </summary>
public static class RandomNumber
{
    /// <summary>
    /// <see cref="Random"/>
    /// </summary>
    private static readonly Random Random = Random.Shared;

    /// <summary>
    /// 随机整数
    /// </summary>
    /// <returns>随机整数：<![CDATA[0<=value<]]><see cref="F:System.Int32.MaxValue">Int32.MaxValue</see></returns>
    public static int Int() => Random.Next();

    /// <summary>
    /// 随机整数
    /// </summary>
    /// <param name="max">最大值</param>
    /// <returns>随机整数：<![CDATA[0<=value<max]]></returns>
    public static int Int(int max) => Random.Next(max);

    /// <summary>
    /// 随机整数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机整数：<![CDATA[min<=value<max]]></returns>
    public static int Int(int min, int max) => Random.Next(min, max);

    /// <summary>
    /// 随机长整数
    /// </summary>
    /// <returns>随机整数：<![CDATA[0<=value<]]><see cref="System.Int64.MaxValue">Int64.MaxValue</see></returns>
    public static long Long() => Random.NextInt64();

    /// <summary>
    /// 随机长整数
    /// </summary>
    /// <param name="max">最大值</param>
    /// <returns>随机整数：<![CDATA[0<=value<max]]></returns>
    public static long Long(long max) => Random.NextInt64(max);

    /// <summary>
    /// 随机长整数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机整数：<![CDATA[min<=value<max]]></returns>
    public static long Long(long min, long max) => Random.NextInt64(min, max);

    /// <summary>
    /// 随机浮点数
    /// </summary>
    /// <returns>随机整数：<![CDATA[0<=value<1.0]]></returns>
    public static double Double() => Random.NextDouble();

    /// <summary>
    /// 随机浮点数
    /// </summary>
    /// <param name="max">最大值</param>
    /// <returns>随机整数：<![CDATA[0<=value<max*1.0]]></returns>
    public static double Double(double max) => Random.NextDouble() * max;

    /// <summary>
    /// 随机浮点数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>随机整数：<![CDATA[min<=value<max * (max - min) + min]]></returns>
    public static double Double(double min, double max) => Random.NextDouble() * (max - min) + min;

    /// <summary>
    /// 数字格式化输出
    /// </summary>
    /// <param name="length">数字长度</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns>固定长度的字符数字</returns>
    public static string Format(int length, long min = 0, long max = long.MaxValue) =>
        Random.NextInt64(min, max).ToString(new string('0', length));
}