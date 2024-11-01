using PostToys.Parse;

namespace PostToys.Post.Database;

public class ParseUrl : AbstractParseUrl
{
    protected override bool TryParseLast(string[] items, out bool hasLast, out string last)
    {
        last = string.Empty;
        hasLast = false;
        if (items.Length < 3)
        {
            return false;
        }

        last = items.Last();
        if (!LastParams.Contains(last.ToLower()))
        {
            return false;
        }

        last = last.ToUpper();
        hasLast = true;
        return true;
    }
}