using PostToys.Parse.Model;

namespace PostToys.Post.Model;

public interface IBoy
{
    /// <summary>
    /// 请求信息
    /// </summary>
    public Toy Toy { get; init; }

    /// <summary>
    /// 请求方法
    /// </summary> 
    public string Method { get; init; }

    /// <summary>
    /// 请求链接
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// 响应内容
    /// </summary>
    public string Body { get; init; }

    /// <summary>
    /// 是否请求成功
    /// </summary>
    public bool IsSuccessStatusCode { get; init; }

    /// <summary>
    /// 响应原因
    /// </summary>
    public string ReasonPhrase { get; init; }

    /// <summary>
    /// 请求耗时，单位：毫秒
    /// </summary>
    public long TakeTime { get; init; }
}