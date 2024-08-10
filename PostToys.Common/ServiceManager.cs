using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace PostToys.Common;

/// <summary>
/// 服务管理器
/// </summary>
public class ServiceManager
{
    /// <summary>
    /// <see cref="IServiceCollection" />
    /// </summary>
    private readonly IServiceCollection _services = new ServiceCollection();

    /// <summary>
    /// <see cref="ServiceProvider" />
    /// </summary>
    private ServiceProvider _provider;

    /// <summary>
    /// 无参构造器
    /// </summary>
    public ServiceManager()
    {
        _provider = _services.BuildServiceProvider();
    }

    /// <summary>
    /// 添加一个单例服务类
    /// </summary>
    /// <param name="serviceKey">服务类的key</param>
    /// <typeparam name="TService">服务类接口</typeparam>
    /// <typeparam name="TImplementation">服务类接口实现类型</typeparam>
    public void AddKeyedSingleton<TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        TImplementation>(
        object? serviceKey)
        where TService : class
        where TImplementation : class, TService
    {
        _provider = _services.AddKeyedSingleton<TService, TImplementation>(serviceKey).BuildServiceProvider();
    }

    /// <summary>
    /// 通过key获取服务类
    /// </summary>
    /// <param name="serviceKey">服务类的key</param>
    /// <typeparam name="T">服务类类型</typeparam>
    /// <returns></returns>
    public T? GetKeyedService<T>(object? serviceKey)
    {
        return _provider.GetKeyedService<T>(serviceKey);
    }
}