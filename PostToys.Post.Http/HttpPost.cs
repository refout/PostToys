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

    private static readonly ParseUrl ParseUrl = new()
    {
        LastPrefix = "HTTP/"
    };

    /// <summary>
    /// http 请求执行方法
    /// </summary>
    /// <param name="toy">请求内容：<see cref="Toy"/></param>
    /// <returns>响应内容：<see cref="IBoy"/></returns>
    public IBoy Post(Toy toy)
    {
        if (!ParseUrl.TryParseUrlExp(toy.Url, out var method, out var requestUri, out var requestVersion))
        {
            return new HttpBoy();
        }

        var httpVersion = VersionParse(requestVersion);
        if (httpVersion == HttpVersion.Unknown)
        {
            return new HttpBoy()
            {
                Toy = toy,
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccessStatusCode = false,
                ReasonPhrase = $"Http Version Unknown: {requestVersion}",
                Version = requestVersion,
                TakeTime = 0
            };
        }

        var requestMessage = new HttpRequestMessage
        {
            Method = new HttpMethod(method),
            RequestUri = BuildUri(requestUri, toy.PathVar, toy.Param),
            Version = httpVersion,
            VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
        };
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
            return new HttpBoy()
            {
                Toy = toy,
                Method = method,
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

        return new HttpBoy
        {
            Toy = toy,
            Url = uri!.ToString(),
            Method = method,
            Body = body,
            RequestHeader = toy.Header,
            ResponseHeader = header,
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
    /// <returns>http 请求 uri：<see cref="Uri"/></returns>
    private static Uri BuildUri(string url, string[] pathVar, Dictionary<string, string> param)
    {
        var builder = new UriBuilder(url);

        if (pathVar.Length > 0)
        {
            builder.Path += (builder.Path.EndsWith('/') ? "" : "/") + string.Join('/', pathVar);
        }

        if (param.Count > 0)
        {
            builder.Query += (string.IsNullOrWhiteSpace(builder.Query)
                ? ""
                : builder.Query.EndsWith('&')
                    ? ""
                    : "&") + param.ToParamsStr();
        }

        return builder.Uri;
    }
}