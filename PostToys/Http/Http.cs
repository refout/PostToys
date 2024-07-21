using System.Diagnostics;
using System.Text;
using PostToys.Parse.Model;

namespace PostToys.Http;

public class Http
{
    private static readonly HttpClient _client = new();

    private Http()
    {
    }

    public static Boy Request(Toy toy)
    {
        StringContent content = new(toy.Body, Encoding.UTF8);

        var url = toy.Url + (toy.Url.EndsWith('/') ? "" : "/");
        if (toy.PathVar.Length > 0)
        {
            var pathVar = string.Join('/', toy.Url);
            url += pathVar;
        }

        if (toy.Param.Count > 0)
        {
            url += '?';
            url += toy.Param
                .Select(it => $"{it.Key}={it.Value}")
                .Aggregate((x, y) => $"{x}&{y}");
        }

        _client.BaseAddress = new Uri(url);

        foreach (var (key, value) in toy.Header)
        {
            _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        if (Version.TryParse(toy.Version, out var result))
        {
            _client.DefaultRequestVersion = result;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestMessage = new HttpRequestMessage(new HttpMethod(toy.Method), toy.Url)
        {
            Content = content
        };
        var task = _client.SendAsync(requestMessage);
        var message = task.Result;

        var body = message.Content.ReadAsStringAsync().Result;
        var header = message.Headers.ToDictionary();
        var takeTime = stopwatch.ElapsedMilliseconds;

        return new Boy();
    }
}