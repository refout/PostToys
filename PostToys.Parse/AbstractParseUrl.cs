namespace PostToys.Parse;

public abstract class AbstractParseUrl
{
    public bool TryParseUrlExp(string input, out string method, out string url, out string last)
    {
        var item = input.Split(" ").Where(it => it.Length > 1).ToArray();

        method = string.Empty;
        url = string.Empty;
        last = string.Empty;
        if (item.Length < 2) return false;

        method = item.First();

        if (!TryParseLast(item, out var hasLast, out last)) return false;

        var urlEndIndex = hasLast ? item.Length - 1 : item.Length;

        for (var i = 1; i < urlEndIndex; i++) url += item[i] + " ";

        url = url.Trim();

        return true;
    }

    protected abstract bool TryParseLast(string[] items, out bool hasLast, out string last);

    public string[] LastParams { get; init; } = [];
    public string LastPrefix { get; init; } = string.Empty;
}