using System.Text.Json;

namespace PostToys.Common;

/// <summary>
/// json 工具
/// </summary>
public static class JsonUtil
{
    /// <summary>
    /// json转对象
    /// </summary>
    /// <param name="json">json 字符串</param>
    /// <typeparam name="T">目标对象范型</typeparam>
    /// <returns>转换后的对象</returns>
    public static T? DeserializeJsonFile<T>(string path)
    {
        var json = path.PathToText();
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json) ?? default;
    }

    /// <summary>
    /// json转对象
    /// </summary>
    /// <param name="json">json 字符串</param>
    /// <typeparam name="T">目标对象范型</typeparam>
    /// <returns>转换后的对象</returns>
    public static T? Deserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(json) ?? default;
    }

    /// <summary>
    /// 判断字符串是否为json
    /// </summary>
    /// <param name="json"></param>
    /// <returns>是否为json</returns>
    public static bool IsJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        json = json.Trim();
        if (!(json.StartsWith('[') && json.EndsWith(']') || json.StartsWith('{') && json.EndsWith('}')))
        {
            return false;
        }

        try
        {
            JsonSerializer.Deserialize<object>(json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}