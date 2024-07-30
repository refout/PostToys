using Microsoft.Extensions.DependencyInjection;
using PostToys.Common;
using PostToys.Parse;
using PostToys.Parse.Markdown;
using PostToys.Post;
using PostToys.Post.Http;
using PostToys.Post.Model;

namespace PostToys;

/// <summary>
/// 执行请求
/// </summary>
public class Runner
{
    /// <summary>
    /// <see cref="IServiceCollection"/>
    /// </summary>
    private readonly IServiceCollection _services = new ServiceCollection();

    /// <summary>
    /// <see cref="ServiceProvider"/>
    /// </summary>
    private readonly ServiceProvider _provider;

    private Runner()
    {
        _services.AddKeyedSingleton<IPost, HttpPost>("HTTP");
        _services.AddKeyedSingleton<IParser, MarkdownParser>("MD");
        _provider = _services.BuildServiceProvider();
    }

    /// <summary>
    /// 添加解析器
    /// </summary>
    /// <param name="key">解析器名称</param>
    /// <typeparam name="TImpl">解析器实现</typeparam>
    public void AddParser<TImpl>(string key) where TImpl : class, IParser
    {
        _services.AddKeyedSingleton<IParser, TImpl>(key);
    }

    /// <summary>
    /// 添加请求处理器
    /// </summary>
    /// <param name="key">请求处理器名称</param>
    /// <typeparam name="TImpl">请求处理器实现</typeparam>
    public void AddPost<TImpl>(string key) where TImpl : class, IPost
    {
        _services.AddKeyedSingleton<IPost, TImpl>(key);
    }

    /// <summary>
    /// 执行请求
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="names">请求名称</param>
    public static void Run(string path, params string[] names)
    {
        var runner = new Runner();
        if (string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        if (names.Length == 0)
        {
            return;
        }

        var extension = path.GetExtensionName();

        var parser = runner._provider.GetKeyedService<IParser>(extension.ToUpper());
        if (parser == null)
        {
            new Boy
            {
                ReasonPhrase = $"Could not find {extension.ToUpper()} parser."
            }.Print();
            return;
        }

        var toys = parser.ToyDictionary(path.PathToLines());
        foreach (var name in names)
        {
            if (toys.TryGetValue(name, out var toy))
            {
                var post = runner._provider.GetKeyedService<IPost>(toy.Type.ToUpper());

                post?.Post(toy).Print();
                continue;
            }

            Console.WriteLine($"Could not find {name}.");
        }
    }
}