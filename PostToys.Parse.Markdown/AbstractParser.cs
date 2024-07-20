using System.Text;
using PostToys.Parse.Markdown.Model;
using PostToys.Parse.Markdown.Processor;
using PostToys.Parse.Model;

namespace PostToys.Parse.Markdown;

/// <summary>
/// 解析器抽象类
/// </summary>
public abstract class AbstractParser(string[] lines) : IParser
{
    #region private field

    /// <summary>
    /// 节点id
    /// </summary>
    private int _id;

    /// <summary>
    /// 输入的 markdown 行
    /// </summary>
    private readonly string[] _lines = lines;

    /// <summary>
    /// 解析出的 markdown 节点
    /// </summary>
    private readonly List<Node> _nodes = [];

    /// <summary>
    /// markdown 节点字典，key：节点id，value：节点
    /// </summary>
    private readonly Dictionary<int, Node> _nodeDictionary = [];

    /// <summary>
    /// 处理器
    /// </summary>
    private readonly Dictionary<string, AbstractProcessor?> _processors = [];

    #endregion

    #region public method

    /// <summary>
    /// 转换为 <see cref="Toy"/> 列表
    /// </summary>
    /// <returns><see cref="Toy"/>列表</returns>
    public List<Toy> Toys()
    {
        Parse();

        if (_nodes.Count == 0) return [];

        return _nodes
            .Where(FindNodeForToyFunc())
            .Select(NodeToToy)
            .ToList();
    }

    /// <summary>
    /// 转换为 <see cref="Toy"/> 字典，key：<see cref="Toy"/> 名称，value：<see cref="Toy"/>  
    /// </summary>
    /// <returns><see cref="Toy"/> 字典</returns>
    public Dictionary<string, Toy> ToyDictionary()
    {
        return Toys().ToDictionary(toy => toy.Name, toy => toy);
    }

    #endregion

    #region protected method

    /// <summary>
    /// 获取节点下一个 id
    /// </summary>
    private int NextId => Interlocked.Increment(ref _id);

    /// <summary>
    /// <see cref="Node"/> 转为字典，key：<see cref="Node"/> id，value：<see cref="Node"/>
    /// </summary>
    /// <param name="reload">是否重新转换</param>
    /// <returns><see cref="Node"/> 字典</returns>
    protected Dictionary<int, Node> ToNodeDictionary(bool reload = false)
    {
        if (reload) _nodeDictionary.Clear();

        if (_nodeDictionary.Count != 0) return _nodeDictionary;

        var nodeDictionary = _nodes.ToDictionary(node => node.Id, node => node);
        foreach (var pair in nodeDictionary) _nodeDictionary.Add(pair.Key, pair.Value);

        return _nodeDictionary;
    }

    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <param name="flag">处理器标识</param>
    /// <param name="processor">处理器</param>
    /// <param name="tryAddAction">处理器添加回调方法</param>
    /// <returns>当前解析器</returns>
    protected AbstractParser AddProcessor(string flag, AbstractProcessor processor, Action<bool, string> tryAddAction)
    {
        var success = _processors.TryAdd(flag, processor);
        tryAddAction.Invoke(success, flag);
        return this;
    }

    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <param name="processors">处理器字典，key：处理器标识，value：处理器</param>
    /// <param name="tryAddAction">处理器添加回调方法</param>
    /// <returns>当前的解析器</returns>
    protected AbstractParser AddProcessor(
        Dictionary<string, AbstractProcessor> processors,
        Action<bool, string> tryAddAction
    )
    {
        foreach (var p in processors)
        {
            var success = _processors.TryAdd(p.Key, p.Value);
            tryAddAction.Invoke(success, p.Key);
        }

        return this;
    }

    /// <summary>
    /// 文件全路径转文件内容行
    /// </summary>
    /// <param name="path">文件全路径</param>
    /// <returns>文件内容行</returns>
    protected static string[] PathToLines(string path)
    {
        return File.ReadAllLines(path, Encoding.UTF8);
    }

    /// <summary>
    /// 文本转文本行
    /// </summary>
    /// <param name="text">文本内容</param>
    /// <param name="lineSeparator">行分隔符，默认为：\r\n</param>
    /// <returns>文本行</returns>
    protected static string[] TextToLines(string text, string lineSeparator = "\r\n")
    {
        return string.IsNullOrWhiteSpace(text) ? [] : text.Split(lineSeparator);
    }

    #endregion

    #region private method

    /// <summary>
    /// 解析方法
    /// </summary>
    private void Parse()
    {
        if (_lines.Length == 0) return;

        for (var i = 0; i < _lines.Length; i++)
        {
            var line = _lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var flag = GetProcessorFlag(line.Trim());
            if (_processors.TryGetValue(flag, out var processor) && processor is not null)
            {
                processor.TryToNode(_nodes, _lines, ref i, NextId);
            }
        }
    }

    #endregion

    # region abstract method

    /// <summary>
    /// 获取处理器标识
    /// </summary>
    /// <param name="line">文本内容</param> 
    /// <returns>处理器标识</returns>
    protected abstract string GetProcessorFlag(string line);

    /// <summary>
    /// <see cref="Node"/> 转 <see cref="Toy"/> 
    /// </summary>
    /// <param name="node"><see cref="Node"/></param>
    /// <returns><see cref="Toy"/></returns>
    protected abstract Toy NodeToToy(Node node);

    /// <summary>
    /// 查找用于转换为 <see cref="Toy"/> 的 <see cref="Node"/>
    /// </summary>
    /// <returns><see cref="Func{Node}"/></returns>
    protected abstract Func<Node, bool> FindNodeForToyFunc();

    #endregion
}