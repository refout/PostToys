using PostToys.Parse.Model;

namespace PostToys.Parse;

public interface IParser
{
    /// <summary>
    /// 转换为 <see cref="Toy"/> 列表
    /// </summary>
    /// <returns><see cref="Toy"/>列表</returns>
    List<Toy> Toys();
    
    /// <summary>
    /// 转换为 <see cref="Toy"/> 字典，key：<see cref="Toy"/> 名称，value：<see cref="Toy"/>  
    /// </summary>
    /// <returns><see cref="Toy"/> 字典</returns>
    Dictionary<string, Toy> ToyDictionary();
}