using PostToys.Parse.Enum;

namespace PostToys.Parse;

public static class UrlExtensions
{
    public static ToyType GetToyType(this string url)
    {
        if (url is { Length: <= 0 })
        {
            throw new InvalidOperationException("Unable to parse URL");
        }

        var item = url.Split(" ").Where(it => it.Length > 1).ToArray();
        return item[0].GetToyTypeByMethod();
    }

    /// <summary>
    /// 通过method获取请求类型 <see cref="ToyType"/>
    /// </summary>
    /// <param name="method">method</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">参数为空异常</exception>
    /// <exception cref="ArgumentException">url类型未知异常</exception>
    public static ToyType GetToyTypeByMethod(this string method)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            throw new ArgumentNullException(nameof(method));
        }

        return method switch
        {
            // GET：请求资源。
            // POST：提交数据。
            // PUT：替换资源。
            // DELETE：删除资源。
            // HEAD：请求响应头信息。
            // OPTIONS：请求支持的方法。
            // PATCH：部分修改资源。
            // CONNECT：建立隧道。
            // TRACE：回显请求。
            "GET" or "POST" or "PUT" or "DELETE" or "HEAD" or "OPTIONS" or "PATCH" or "CONNECT" or "TRACE" => ToyType
                .Http,
            // DDL（数据定义语言）：用于定义和管理数据库结构，主要命令有 CREATE, ALTER, DROP, TRUNCATE, RENAME。
            // DML（数据操纵语言）：用于操作数据库中的数据，主要命令有 SELECT, INSERT, UPDATE, DELETE。
            // DCL（数据控制语言）：用于控制对数据库的访问权限，主要命令有 GRANT, REVOKE。
            // DQL（数据查询语言）：主要用于查询数据库中的数据，主要命令有 SELECT
            "DDL" or "DML" or "DCL" or "DQL" => ToyType.Database,
            _ => throw new ArgumentException("Invalid method", nameof(method)),
        };
    }
}