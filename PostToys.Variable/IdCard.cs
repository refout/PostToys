using System.Diagnostics.CodeAnalysis;

namespace PostToys.Variable;

/// <summary>
/// 身份证号生成
/// </summary>
public class IdCard
{
    /// <summary>
    /// 公民身份号码中各个位置上的加权因子W数值
    /// </summary>
    private static readonly int[] W = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];

    /// <summary>
    /// 校验码字符值换算关系
    /// </summary>
    private static readonly char[] A = ['1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2'];

    /// <summary>
    /// 地址码：表示编码对象常住户口所在县(市、旗、区)的行政区划代码，按GB/T2260的规定执行。
    /// </summary>
    private readonly AddressCode _address;

    /// <summary>
    /// 出生日期码：表示编码对象出生的年、月、日，按GB/T7408的规定执行，年、月、日分别用4位、2位（不足两位加0）、2位（同上）数字表示，之间不用分隔符
    /// </summary>
    private readonly BirthdayCode _birthday;

    /// <summary>
    /// 顺序码：表示在同一地址码所标识的区域范围内，对同年、同月、同日出生的人编定的顺序号，顺序码的奇数分配给男性，偶数分配给女性
    /// </summary>
    private readonly int _sequence;

    /// <summary>
    /// 私有构造
    /// </summary>
    /// <param name="address"><see cref="AddressCode"/></param>
    /// <param name="birthday"><see cref="BirthdayCode"/></param>
    /// <param name="sequence">顺序码</param>
    private IdCard(AddressCode address, BirthdayCode birthday, int sequence)
    {
        _address = address;
        _birthday = birthday;
        _sequence = sequence;
    }

    /// <summary>
    /// 生成证件号
    /// </summary>
    /// <param name="address"><see cref="AddressCode"/></param>
    /// <param name="birthday"><see cref="BirthdayCode"/></param>
    /// <param name="sequence">顺序码</param>
    /// <returns><see cref="IdCard"/></returns>
    /// <exception cref="ArgumentOutOfRangeException">顺序码范围错误</exception>
    public static IdCard Generate(AddressCode address, BirthdayCode birthday, int sequence)
    {
        if (sequence is > 1000 or < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sequence), sequence,
                "Sequence must be greater than 0 and less than 1000");
        }

        return new IdCard(address, birthday, sequence);
    }

    /// <summary>
    /// 生成证件号
    /// </summary>
    public string IdCardNumber => $"{_address}{_birthday}{_sequence:000}{Checksum}";

    /// <summary>
    /// 顺序码的奇数分配给男性，偶数分配给女性
    /// </summary>
    public Genders Gender => _sequence % 2 == 0 ? Genders.Female : Genders.Male;

    /// <summary>
    /// 获取证件号中的地址
    /// </summary>
    public string Address => _address.ToString();

    /// <summary>
    /// 获取证件号生日
    /// </summary>
    public string Birthday => _birthday.ToString();

    /// <summary>
    /// 获取证件号生日
    /// </summary>
    public DateTime BirthdayDateTime => new(_birthday.Year, _birthday.Month, _birthday.Day);

    /// <summary>
    /// 获取证件号生日
    /// </summary>
    public string BirthdayFormat => _birthday.Format();

    /// <summary>
    /// 获取证件号生日年份
    /// </summary>
    public string BirthdayYear => _birthday.Year.ToString("0000");

    /// <summary>
    /// 获取证件号生日月份
    /// </summary>
    public string BirthdayMonth => _birthday.Month.ToString("00");

    /// <summary>
    /// 获取证件号生日天
    /// </summary>
    public string BirthdayDay => _birthday.Day.ToString("00");

    /// <summary>
    /// 获取校验码
    /// </summary>
    /// <exception cref="ArgumentException">ID card checksum is invalid</exception>
    private string Checksum => CalculateChecksum($"{_address}{_birthday}{_sequence:000}");

    /// <summary>
    /// 证件号校验
    /// </summary>
    /// <param name="code">证件号</param>
    /// <returns>是否校验通过</returns>
    public static bool IsValid(string code)
    {
        try
        {
            var checksum = CalculateChecksum(code);
            return code.EndsWith(checksum);
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 计算证件号校验码
    /// </summary>
    /// <param name="code">证件号</param>
    /// <returns>证件号校验码</returns>
    /// <exception cref="FormatException">证件号格式异常</exception>
    /// <exception cref="ArgumentOutOfRangeException">校验码字符值换算关系超场</exception>
    public static string CalculateChecksum(string code)
    {
        var checksum = 0;
        if (code.Length < 17)
        {
            throw new FormatException(
                $"Error formatting id card content {code}, the length of the content must be greater than 17 characters, but it was {code.Length}");
        }

        for (var i = 0; i < 17; i++)
        {
            var a = int.Parse(code[i].ToString());
            checksum += W[i] * a;
        }

        var check = checksum % 11;
        if (check > A.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(code), check, "ID card checksum is invalid");
        }

        return A[check].ToString();
    }

    /// <summary>
    /// 地址码：表示编码对象常住户口所在县(市、旗、区)的行政区划代码，按GB/T2260的规定执行。
    /// </summary>
    /// <param name="Province">省市</param>
    /// <param name="City">地区</param>
    /// <param name="District">县区</param>
    public readonly record struct AddressCode(string Province, string City, string District)
    {
        public override string ToString()
        {
            return Province + City + District;
        }
    }

    /// <summary>
    /// 出生日期码：表示编码对象出生的年、月、日，按GB/T7408的规定执行，年、月、日分别用4位、2位（不足两位加0）、2位（同上）数字表示，之间不用分隔符
    /// </summary>
    /// <param name="Year">年</param>
    /// <param name="Month">月</param>
    /// <param name="Day">日</param>
    public readonly record struct BirthdayCode(int Year, int Month, int Day)
    {
        public override string ToString()
        {
            return $"{Year:0000}{Month:00}{Day:00}";
        }

        /// <summary>
        /// 日期格式化
        /// </summary>
        /// <param name="format">日期格式</param>
        /// <returns>格式化后的日期</returns>
        public string Format([StringSyntax("DateTimeFormat")] string format = "yyyy-MM-dd")
        {
            return new DateTime(Year, Month, Day).ToString(format);
        }
    }

    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum Genders
    {
        /// <summary>
        /// 男
        /// </summary>
        Male,

        /// <summary>
        /// 女
        /// </summary>
        Female
    }
}