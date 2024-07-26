using PostToys.Parse.Model;
using PostToys.Post.Model;

namespace PostToys.Post;

/// <summary>
/// 请求接口
/// </summary>
public interface IPost
{
    /// <summary>
    /// 请求执行方法
    /// </summary>
    /// <param name="toy">请求内容：<see cref="Toy"/></param>
    /// <returns>响应内容：<see cref="Boy"/></returns>
    Boy Post(Toy toy);
}