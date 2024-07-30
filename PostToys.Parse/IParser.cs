using PostToys.Parse.Model;

namespace PostToys.Parse;

/// <summary>
/// 解析器接口
/// </summary>
public interface IParser
{
    /// <summary>
    /// 转换为 <see cref="Toy"/> 列表
    /// </summary>
    /// <param name="lines">输入的文本行</param>
    /// <returns><see cref="Toy"/>列表</returns>
    List<Toy> Toys(string[] lines);

    /// <summary>
    /// 转换为 <see cref="Toy"/> 字典，key：<see cref="Toy"/> 名称，value：<see cref="Toy"/>  
    /// </summary>
    /// <param name="lines">输入的文本行</param>
    /// <returns><see cref="Toy"/> 字典</returns>
    public sealed Dictionary<string, Toy> ToyDictionary(string[] lines)
    {
        return Toys(lines).ToDictionary(toy => toy.Name, toy => toy);
    }
}