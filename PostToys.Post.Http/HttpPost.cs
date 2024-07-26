using System.Diagnostics;
using System.Net;
using System.Text;
using PostToys.Parse.Model;
using PostToys.Post.Model;

namespace PostToys.Post.Http;

/// <summary>
/// http请求实现
/// </summary>
public class HttpPost : IPost
{
    /// <summary>
    /// http 客户端
    /// </summary>
    private static readonly HttpClient Client = new()
    {
        DefaultRequestVersion = HttpVersion.Version11,
        DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact
    };

    /// <summary>
    /// http 请求执行方法
    /// </summary>
    /// <param name="toy">请求内容：<see cref="Toy"/></param>
    /// <returns>响应内容：<see cref="Boy"/></returns>
    public Boy Post(Toy toy)
    {
        var httpVersion = VersionParse(toy.Version);
        if (httpVersion == HttpVersion.Unknown)
        {
            return new Boy()
            {
                Toy = toy,
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccessStatusCode = false,
                ReasonPhrase = $"Http Version Unknown: {toy.Version}",
                Version = toy.Version,
                TakeTime = 0
            };
        }

        var requestMessage = new HttpRequestMessage();
        requestMessage.Method = new HttpMethod(toy.Method);
        requestMessage.RequestUri = BuildUri(toy);
        requestMessage.Version = httpVersion;
        requestMessage.VersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
        foreach (var (key, value) in toy.Header)
        {
            requestMessage.Headers.TryAddWithoutValidation(key, value);
        }

        if (!string.IsNullOrWhiteSpace(toy.Body))
        {
            StringContent content = new(toy.Body, Encoding.UTF8);
            requestMessage.Content = content;
        }

        var stopwatch = Stopwatch.StartNew();
        HttpResponseMessage send;
        try
        {
            var sendAsync = Client.SendAsync(requestMessage);
            send = sendAsync.Result;
        }
        catch (Exception e)
        {
            return new Boy()
            {
                Toy = toy,
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccessStatusCode = false,
                ReasonPhrase = e.Message,
                Version = VersionParse(httpVersion),
                TakeTime = stopwatch.ElapsedMilliseconds
            };
        }

        var takeTime = stopwatch.ElapsedMilliseconds;
        var uri = send.RequestMessage?.RequestUri;
        var version = send.Version;
        var statusCode = send.StatusCode;
        var phrase = send.ReasonPhrase;
        var successStatusCode = send.IsSuccessStatusCode;
        var header = send.Headers.ToDictionary();
        var body = send.Content.ReadAsStringAsync().Result;

        return new Boy()
        {
            Toy = toy,
            Uri = uri,
            Body = body,
            Header = header,
            StatusCode = statusCode,
            IsSuccessStatusCode = successStatusCode,
            ReasonPhrase = string.IsNullOrWhiteSpace(phrase) ? "REQUEST SUCCESS" : phrase,
            Version = VersionParse(version),
            TakeTime = takeTime
        };
    }

    /// <summary>
    /// http 版本号解析为 <see cref="Version"/>
    /// </summary>
    /// <param name="version">http 版本号，如：HTTP/1.1</param>
    /// <returns></returns>
    private static Version VersionParse(string version)
    {
        return version switch
        {
            "HTTP/1.0" => HttpVersion.Version10,
            "HTTP/1" => HttpVersion.Version10,
            "HTTP/1.1" => HttpVersion.Version11,
            "HTTP/2.0" => HttpVersion.Version20,
            "HTTP/2" => HttpVersion.Version20,
            "HTTP/3.0" => HttpVersion.Version30,
            "HTTP/3" => HttpVersion.Version30,
            _ => HttpVersion.Unknown
        };
    }

    /// <summary>
    /// <see cref="Version"/> 转 http 版本字符串，如：HTTP/1.1
    /// </summary>
    /// <param name="version"><see cref="Version"/></param>
    /// <returns>http 版本字符串，如：HTTP/1.1</returns>
    private static string VersionParse(Version version)
    {
        return $"HTTP/{version}";
    }

    /// <summary>
    /// http 请求 uri 构建
    /// </summary>
    /// <param name="toy">请求信息：<see cref="Toy"/></param>
    /// <returns>http 请求 uri：<see cref="Uri"/></returns>
    private static Uri BuildUri(Toy toy)
    {
        var builder = new UriBuilder(toy.Url);

        if (toy.PathVar.Length > 0)
        {
            builder.Path += (builder.Path.EndsWith('/') ? "" : "/") + string.Join('/', toy.PathVar);
        }

        if (toy.Param.Count > 0)
        {
            builder.Query += (string.IsNullOrWhiteSpace(builder.Query)
                ? ""
                : builder.Query.EndsWith('&')
                    ? ""
                    : "&") + toy.Param.ToParamsStr();
        }

        return builder.Uri;
    }
}