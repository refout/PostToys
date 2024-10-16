using System.Diagnostics.CodeAnalysis;
using PostToys.Common;
using PostToys.Expression;
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
    private static readonly ExpressionParse parse = new();

    /// <summary> 执行请求 </summary>
    /// <param name="path">文件路径</param>
    /// <param name="names">请求名称</param>
    public static Runner Run(string path, string? envPath, params string[] names)
    {
        var runner = new Runner();
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

        if (names.Length == 0) throw new ArgumentNullException(nameof(names));

        Dictionary<string, string> env = [];
        if (!string.IsNullOrWhiteSpace(envPath))
        {
            env = JsonUtil.DeserializeJsonFile<Dictionary<string, object>>(envPath)?
            .ToDictionary(k => k.Key, v => v.Value?.ToString() ?? "") ?? [];
        }

        var toys = ToyParser.Parse(path);

        foreach (var name in names)
        {
            if (toys.TryGetValue(name, out var toy))
            {
                toy = Parse(toy, env);
                var boy = PostManger.Post(toy);
                boy.Print();
                continue;
            }

            Console.WriteLine($"Could not find {name}.");
        }

        return runner;
    }

    /// <summary>
    /// 解析表达式并替换为值
    /// </summary>
    /// <param name="toy">请求信息</param>
    /// <param name="env">环境变量</param>
    /// <returns>替换表达式值后的请求信息</returns>
    private static Toy Parse(Toy toy, Dictionary<string, string> env)
    {
        env = Parse(env, env);
        return toy with
        {
            Url = Parse(toy.Url, env),
            Header = Parse(toy.Header, env),
            Param = Parse(toy.Param, env),
            PathVar = Parse(toy.PathVar, env),
            Body = Parse(toy.Body, env),
        };
    }

    /// <summary>
    /// 解析表达式并替换为值
    /// </summary>
    /// <param name="pairs">含表达式的字典</param>
    /// <param name="env">环境变量</param>
    /// <returns>替换表达式值后的字典</returns>
    private static Dictionary<string, string> Parse(Dictionary<string, string> pairs, Dictionary<string, string> env)
    {
        if (pairs is { Count: <= 0 })
        {
            return pairs;
        }

        foreach (var pair in pairs)
        {
            pairs[pair.Key] = Parse(pair.Value, env);
        }
        return pairs;
    }

    /// <summary>
    /// 解析表达式并替换为值
    /// </summary>
    /// <param name="items">含表达式的数组</param>
    /// <param name="env">环境变量</param>
    /// <returns>替换表达式值后的数组</returns>
    private static string[] Parse(string[] items, Dictionary<string, string> env)
    {
        if (items is { Length: <= 0 })
        {
            return items;
        }

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = Parse(items[i], env);
        }
        return items;
    }

    /// <summary>
    /// 解析表达式并替换为值
    /// </summary>
    /// <param name="text">含表达式的文本</param>
    /// <param name="env">环境变量</param>
    /// <returns>替换表达式值后的文本</returns>
    private static string Parse(string text, Dictionary<string, string> env)
    {
        if (text.Length == 0)
        {
            return text;
        }

        if (parse.TryParseValue(text, env, out List<(string fullexpression, string result)> expressions))
        {
            if (expressions == null || expressions.Count == 0)
            {
                return text;
            }

            expressions.ForEach(it => text = text.Replace(it.fullexpression, it.result));
        }

        return text;
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

        var parser = _serviceManager.GetKeyedService<IParser>(extension) ??
        throw new ArgumentException($"Invalid file type: {extension}.");
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
        var post = _serviceManager.GetKeyedService<IPost>(toy.Type) ??
        throw new ArgumentException($"Invalid toy type: {toy.Type}.");
        return post.Post(toy);
    }
}

#endregion