namespace PostToys.Post.Http;

/// <summary>
/// http扩展方法
/// </summary>
public static class HttpExtensions
{
    /// <summary>
    /// 请求参数转字符串
    /// </summary>
    /// <param name="param">请求参数</param>
    /// <returns>请求参数字符串</returns>
    public static string ToParamsStr(this Dictionary<string, string> param)
    {
        return param
            .Select(it => $"{it.Key}={it.Value}")
            .Aggregate((x, y) => $"{x}&{y}");
    }
}