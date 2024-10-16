using System.Text;

namespace PostToys.Common;

/// <summary>
/// 文件相关扩展方法
/// </summary>
public static class FileExtensions
{
    /// <summary>
    /// 文件全路径转文件内容行
    /// </summary>
    /// <param name="path">文件全路径</param>
    /// <returns>文件内容行</returns>
    public static string[] PathToLines(this string path)
    {
        return File.ReadAllLines(path, Encoding.UTF8);
    }

    /// <summary>
    /// 文件全路径转文件内容
    /// </summary>
    /// <param name="path">文件全路径</param>
    /// <returns>文件内容</returns>
    public static string PathToText(this string path)
    {
        return File.ReadAllText(path, Encoding.UTF8);
    }
    
    /// <summary>
    /// 文本转文本行
    /// </summary>
    /// <param name="text">文本内容</param>
    /// <param name="lineSeparator">行分隔符，默认为：\r\n</param>
    /// <returns>文本行</returns>
    public static string[] TextToLines(this string text, string lineSeparator = "\r\n")
    {
        return string.IsNullOrWhiteSpace(text) ? [] : text.Split(lineSeparator);
    }

    /// <summary>
    /// 根据文件名获取文件后缀名
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns></returns>
    public static string GetExtensionName(this string filename)
    {
        var extension = Path.GetExtension(filename);
        if (string.IsNullOrEmpty(extension))
        {
            return string.Empty;
        }

        extension = extension.TrimStart('.');
        return extension;
    }
}