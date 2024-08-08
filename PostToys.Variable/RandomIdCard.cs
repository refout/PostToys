namespace PostToys.Variable;

/// <summary>
/// 生成随机证件号
/// </summary>
public static class RandomIdCard
{
    /// <summary>
    /// 出生日期最小年份
    /// </summary>
    private const int MinYear = 1900;

    /// <summary>
    /// 出生日期最小月份
    /// </summary>
    private const int MinMonth = 1;

    /// <summary>
    /// 出生日期最小日期
    /// </summary>
    private const int MinDay = 1;

    /// <summary>
    /// 随机地址
    /// </summary>
    private static IdCard.AddressCode RandomAddressCode => new(
        RandomNumber.Format(2, 0, 99),
        RandomNumber.Format(2, 0, 99),
        RandomNumber.Format(2, 0, 99)
    );

    /// <summary>
    /// 随机出生日期
    /// </summary>
    private static IdCard.BirthdayCode RandomBirthdayCode
    {
        get
        {
            var (year, month, day) = RandomDateTime.Range(new DateTime(MinYear, MinMonth, MinDay), DateTime.Today);
            return new IdCard.BirthdayCode(year, month, day);
        }
    }

    /// <summary>
    /// 随机顺序码
    /// </summary>
    private static int RandomSequence => RandomNumber.Int(1000);

    /// <summary>
    /// 生成一个随机证件号
    /// </summary>
    public static string Number =>
        IdCard.Generate(RandomAddressCode, RandomBirthdayCode, RandomSequence).IdCardNumber;

    /// <summary>
    /// 生成一个随机证件号对象 <see cref="IdCard"/>
    /// </summary>
    public static IdCard IdCardInfo(
        string? address = default,
        int year = MinYear, int month = MinMonth, int day = MinDay)
    {
        var addressCode = address == default
            ? RandomAddressCode
            : address.Length != 6
                ? throw new InvalidDataException("Invalid address code, it must be 6 digits")
                : new IdCard.AddressCode(address[..2], address[2..4], address[4..]);
        var birthday = year == MinYear && month == MinMonth && day == MinDay
            ? RandomBirthdayCode
            : new IdCard.BirthdayCode(year, month, day);
        return IdCard.Generate(addressCode, birthday, RandomSequence);
    }
}