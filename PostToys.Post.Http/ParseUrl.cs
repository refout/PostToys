using PostToys.Parse;

namespace PostToys.Post.Http;

public class ParseUrl : AbstractParseUrl
{
    protected override bool TryParseLast(string[] items, out bool hasLast, out string last)
    {
        last = "HTTP/1.1";
        hasLast = false;
        if (items.Length < 3)
        {
            return true;
        }

        last = items.Last();
        
        if (!last.StartsWith(LastPrefix, StringComparison.CurrentCultureIgnoreCase)) return false;
        
        last = last.ToUpper();
        hasLast = true;
        return true;
    }
}