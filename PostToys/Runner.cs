using System.Diagnostics.CodeAnalysis;
using PostToys.Common;
using PostToys.Parse;
using PostToys.Parse.Enum;
using PostToys.Parse.Markdown;
using PostToys.Parse.Model;
using PostToys.Post;
using PostToys.Post.Http;
using PostToys.Post.Model;

namespace PostToys;

/// <summary>
/// 执行请求
/// </summary>
public class Runner
{
    private static readonly PostManger PostManger = new();
    private static readonly ToyParser ToyParser = new();

    /// <summary> 执行请求 </summary>
    /// <param name="path">文件路径</param>
    /// <param name="names">请求名称</param>
    public static Runner Run(string path, params string[] names)
    {
        var runner = new Runner();
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        if (names.Length == 0) throw new ArgumentNullException(nameof(names));

        var toys = ToyParser.Parse(path);

        foreach (var name in names)
        {
            if (toys.TryGetValue(name, out var toy))
            {
                var boy = PostManger.Post(toy);
                boy.Print();
                continue;
            }

            Console.WriteLine($"Could not find {name}.");
        }

        return runner;
    }
}

#region 接口服务类

/// <summary>
/// 解析器管理
/// </summary>
public class ToyParser
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    private readonly ServiceManager _serviceManager = new();

    /// <summary>
    /// 构造器
    /// </summary>
    public ToyParser()
    {
        _serviceManager.AddKeyedSingleton<IParser, MarkdownParser>(ParserType.Markdown.Extension());
    }

    /// <summary>
    /// 执行解析
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>请求信息</returns>
    /// <exception cref="ArgumentNullException">文件路径为空异常</exception>
    /// <exception cref="ArgumentException">获取解析器异常</exception>
    public Dictionary<string, Toy> Parse(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        var extension = path.GetExtensionName().Trim().ToLower();

        var parser = _serviceManager.GetKeyedService<IParser>(extension);
        if (parser == null)
        {
            throw new ArgumentException($"Invalid file type: {extension}.");
        }

        return parser.ToyDictionary(path.PathToLines());
    }

    /// <summary>
    /// 添加解析器
    /// </summary>
    /// <param name="serviceKey">解析器key</param>
    /// <typeparam name="TImplementation">解析器实现类型</typeparam>
    public void AddParser
        <[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>
        (object? serviceKey) where TImplementation : class, IParser

    {
        _serviceManager.AddKeyedSingleton<IParser, TImplementation>(serviceKey);
    }
}

/// <summary>
/// 请求管理器
/// </summary>
public class PostManger
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    private readonly ServiceManager _serviceManager = new();

    /// <summary>
    /// 构造器
    /// </summary>
    public PostManger()
    {
        _serviceManager.AddKeyedSingleton<IPost, HttpPost>(ToyType.Http);
    }

    /// <summary>
    /// 执行请求
    /// </summary>
    /// <param name="toy">请求信息</param>
    /// <returns>响应信息</returns>
    /// <exception cref="ArgumentException">获取请求接口为空</exception>
    public Boy Post(Toy toy)
    {
        var post = _serviceManager.GetKeyedService<IPost>(toy.Type);
        if (post == null)
        {
            throw new ArgumentException($"Invalid toy type: {toy.Type}.");
        }

        return post.Post(toy);
    }
}

#endregion