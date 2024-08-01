using System.Text;

namespace PostToys.Variable;

/// <summary>
/// 随机字母
/// </summary>
public static class RandomLetter
{
    /// <summary>
    /// 小写字母集合
    /// </summary>
    private static readonly char[] LowerLetters = new char[26];

    /// <summary>
    /// 大写字母集合
    /// </summary>
    private static readonly char[] UpperLetters = new char[26];

    /// <summary>
    /// 静态构造
    /// </summary>
    static RandomLetter()
    {
        // 字母集合初始化
        for (var i = 0; i < 26; i++)
        {
            LowerLetters[i] = (char)(i + 97);
            UpperLetters[i] = (char)(i + 65);
        }
    }

    /// <summary>
    /// 生成随机字母
    /// </summary>
    /// <param name="count">字母数量</param>
    /// <param name="letterCase">字母大小写选项：<see cref="LetterCase"/></param>
    /// <returns></returns>
    public static string Letter(int count, LetterCase letterCase = LetterCase.Random)
    {
        var builder = new StringBuilder();

        var letters = letterCase switch
        {
            LetterCase.Lower => LowerLetters,
            LetterCase.Upper => UpperLetters,
            LetterCase.Random => [..LowerLetters, ..UpperLetters],
            _ => []
        };
        for (var i = 0; i < count; i++)
        {
            builder.Append(letters[RandomNumber.Int(0, letters.Length - 1)]);
        }

        return builder.ToString();
    }

    /// <summary>
    /// 字母大小写选项
    /// </summary>
    public enum LetterCase
    {
        /// <summary>
        /// 大写
        /// </summary>
        Upper,

        /// <summary>
        /// 小写
        /// </summary>
        Lower,

        /// <summary>
        /// 大小写随机
        /// </summary>
        Random,
    }
}