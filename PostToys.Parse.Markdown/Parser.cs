﻿using System.Text.Json;
using PostToys.Parse.Markdown.Model;
using PostToys.Parse.Markdown.ParseHandler;
using PostToys.Parse.Model;

namespace PostToys.Parse.Markdown;

/// <summary>
/// markdown 解析器
/// </summary>
public class Parser : AbstractParser
{
    /// <summary>
    /// 私有构造器
    /// </summary>
    /// <param name="lines"></param>
    private Parser(string[] lines) : base(lines)
    {
        // 添加默认处理器
        AddProcessor(MarkdownFlag.Header, new ParseHeader());
        AddProcessor(MarkdownFlag.Blockquote, new ParseBlockquote());
        AddProcessor(MarkdownFlag.CodeBlock, new ParseCodeBlock());
        AddProcessor(MarkdownFlag.Paragraph, new ParseParagraph());
    }

    /// <summary>
    /// 输入行
    /// </summary>
    /// <param name="lines">行</param>
    /// <returns>当前解析器</returns>
    public static Parser InputLines(string[] lines)
    {
        return new Parser(lines);
    }

    /// <summary>
    /// 输入文件路径
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>当前解析器</returns>
    public static Parser InputPath(string path)
    {
        return InputLines(PathToLines(path));
    }

    /// <summary>
    /// 输入文本内容
    /// </summary>
    /// <param name="text">文本内容</param>
    /// <param name="lineSeparator">行分隔符，默认为：\r\n</param>
    /// <returns>文本行</returns>
    public static Parser InputText(string text, string lineSeparator = "\r\n")
    {
        return new Parser(TextToLines(text, lineSeparator));
    }


    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <param name="flag">处理器标志</param>
    /// <param name="processor">处理器</param>
    /// <returns>当前解析器</returns>
    public Parser AddProcessor(string flag, AbstractProcessor processor)
    {
        AddProcessor(flag, processor, (success, s) =>
        {
            if (success) MarkdownFlag.AddFlag(s);
        });
        return this;
    }

    /// <summary>
    /// 添加处理器
    /// </summary>
    /// <param name="processors">处理器字典，key：处理器标志，value：处理器</param>
    /// <returns>当前的解析器</returns>
    public Parser AddProcessor(Dictionary<string, AbstractProcessor> processors)
    {
        AddProcessor(processors, (success, s) =>
        {
            if (success) MarkdownFlag.AddFlag(s);
        });

        return this;
    }

    /// <summary>
    /// 获取处理器标识
    /// </summary>
    /// <param name="line">文本内容</param> 
    /// <returns>处理器标识</returns>
    protected override string GetProcessorFlag(string line)
    {
        return MarkdownFlag.ToFlag(line.Trim());
    }

    /// <summary>
    /// 查找用于转换为 <see cref="Toy"/> 的 <see cref="Node"/>
    /// </summary>
    /// <returns><see cref="Func{Node}"/></returns>
    protected override Func<Node, bool> FindNodeForToyFunc()
    {
        return node => node is Header { Level: 2 } && node.ChildrenId is { Count: > 0 };
    }

    /// <summary>
    /// <see cref="Node"/> 转 <see cref="Toy"/> 
    /// </summary>
    /// <param name="request"><see cref="Node"/></param>
    /// <returns><see cref="Toy"/></returns>
    protected override Toy NodeToToy(Node request)
    {
        var nodeDictionary = ToNodeDictionary();

        nodeDictionary.Remove(request.Id);

        var requestItems =
            from childId in request.ChildrenId
            select nodeDictionary[childId]
            into node
            where node.ChildrenId is { Count: > 0 } && node is Header { Level: 3 }
            select node;

        var url = "";
        var method = "";
        var version = "";
        var header = new Dictionary<string, object>();
        var param = new Dictionary<string, object>();
        var pathVar = Array.Empty<object>();
        var body = "";

        foreach (var item in requestItems)
        {
            nodeDictionary.Remove(item.Id);

            var itemChildId = item.ChildrenId![0];

            var child = nodeDictionary[itemChildId];

            nodeDictionary.Remove(itemChildId);

            var content = item.Content;
            switch (child)
            {
                case CodeBlock code when content.Contains("body"):
                    body = code.Lang switch
                    {
                        "json" => code.Content,
                        _ => ""
                    };
                    break;
                case CodeBlock code when content.Contains("param"):
                    param = code.Lang switch
                    {
                        "json" => JsonSerializer.Deserialize<Dictionary<string, object>>(code.Content) ?? [],
                        _ => []
                    };
                    break;
                case CodeBlock code when content.Contains("pathVar"):
                    pathVar = code.Lang switch
                    {
                        "json" => JsonSerializer.Deserialize<object[]>(code.Content) ?? [],
                        _ => []
                    };
                    break;
                case CodeBlock code when content.Contains("header"):
                    header = code.Lang switch
                    {
                        "json" => JsonSerializer.Deserialize<Dictionary<string, object>>(code.Content) ?? [],
                        _ => []
                    };
                    break;
                case Blockquote blockquote when content.Contains("url"):
                    if (TryParseUrlExp(blockquote.Content, out var m, out var u, out var v))
                    {
                        method = m;
                        url = u;
                        version = v;
                    }

                    break;
            }
        }

        return new Toy
        {
            Name = (request.ParentId == null
                ? ""
                : nodeDictionary[(int)request.ParentId].Content + "@") + request.Content,
            Url = url,
            Method = method,
            Version = version,
            Header = header,
            Param = param,
            PathVar = pathVar,
            Body = body
        };
    }

    /// <summary>
    /// 尝试解析链接表达式，格式为：<code>POST http://{ip}:{{port}}{{context-path}}{{baseUrl}}/create HTTP/1.1</code>
    /// </summary>
    /// <param name="input">输入内容</param>
    /// <param name="method">http请求方法，即是第一部分，必须包含</param>
    /// <param name="url">http链接，即是第二部分，必须包含</param>
    /// <param name="version">http版本，即是第三部分，可选，默认为HTTP/1.1</param>
    /// <returns>是否解析成功</returns>
    private static bool TryParseUrlExp(string input, out string method, out string url, out string version)
    {
        var item = input.Split(" ").Where(it => it.Length > 1).ToArray();

        method = "";
        url = "";
        version = "";
        if (item.Length < 2) return false;

        if (!item[1].StartsWith("http")) return false;

        method = item.First();
        version = "HTTP/1.1";
        if (item.Length < 3)
        {
            url = item[1];
            return true;
        }

        var lastItem = item.Last();
        if (lastItem.StartsWith("HTTP/", StringComparison.CurrentCultureIgnoreCase)) version = lastItem.ToUpper();

        for (var i = 1; i < item.Length - 1; i++) url += item[i] + " ";

        url = url.Trim();

        return true;
    }
}