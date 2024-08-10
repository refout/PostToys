namespace PostToys.Parse.Enum;

/// <summary>
/// 请求类型
/// </summary>
public enum ToyType
{
    /// <summary>
    /// http请求
    /// </summary>
    Http
}

/// <summary>
/// <see cref="ToyType"/> 扩展类
/// </summary>
public static class ToyTypeExtensions
{
    /// <summary>
    /// 通过url获取请求类型 <see cref="ToyType"/>
    /// </summary>
    /// <param name="url">url</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">参数为空异常</exception>
    /// <exception cref="ArgumentException">url类型未知异常</exception>
    public static ToyType GetToyTypeByUrl(this string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentNullException(nameof(url));
        }

        if (url.StartsWith("http"))
        {
            return ToyType.Http;
        }

        throw new ArgumentException("Invalid url", nameof(url));
    }
}