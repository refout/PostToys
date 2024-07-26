using System.Net;
using PostToys.Parse.Model;

namespace PostToys.Post.Model;

/// <summary>
/// 请求响应
/// </summary>
public record struct Boy
{
    /// <summary>
    /// 请求信息
    /// </summary>
    public Toy Toy { get; init; }

    /// <summary>
    /// 请求链接
    /// </summary>
    public Uri? Uri { get; init; }

    /// <summary>
    /// 响应内容
    /// </summary>
    public string Body { get; init; }

    /// <summary>
    /// 响应内容
    /// </summary>
    public Dictionary<string, IEnumerable<string>> Header { get; init; }

    /// <summary>
    /// 响应状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// 是否请求成功
    /// </summary>
    public bool IsSuccessStatusCode { get; init; }

    /// <summary>
    /// 响应原因
    /// </summary>
    public string ReasonPhrase { get; init; }

    /// <summary>
    /// 实际请求版本信息
    /// </summary>
    public string Version { get; init; }

    /// <summary>
    /// 请求耗时，单位：毫秒
    /// </summary>
    public long TakeTime { get; init; }
}