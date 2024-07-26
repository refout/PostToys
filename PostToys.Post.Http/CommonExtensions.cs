namespace PostToys.Post.Http;

/// <summary>
/// 公告扩展方法
/// </summary>
public static class CommonExtensions
{
    /// <summary>
    /// 请求参数转字符串
    /// </summary>
    /// <param name="param">请求参数</param>
    /// <returns>请求参数字符串</returns>
    public static string ToParamsStr(this Dictionary<string, object> param)
    {
        return param
            .Select(it => $"{it.Key}={it.Value}")
            .Aggregate((x, y) => $"{x}&{y}");
    }
}