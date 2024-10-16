using PostToys.Parse.Enum;

namespace PostToys.Parse.Model;

/// <summary>
/// 请求信息；
/// POST 的一个载体
/// </summary>
public readonly record struct Toy
{
    /// <summary>
    /// 类型
    /// </summary>
    public ToyType Type { get => Url.GetToyTypeByUrl(); }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// 链接
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// 请求方法
    /// </summary>
    public string Method { get; init; }

    /// <summary>
    /// HTTP 版本
    /// </summary>
    public string Version { get; init; }

    /// <summary>
    /// 请求头
    /// </summary>
    public Dictionary<string, string> Header { get; init; }

    /// <summary>
    /// 请求参数
    /// </summary>
    public Dictionary<string, string> Param { get; init; }

    /// <summary>
    /// 路径参数
    /// </summary>
    public string[] PathVar { get; init; }

    /// <summary>
    /// 请求体
    /// </summary>
    public string Body { get; init; }
}