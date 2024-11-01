﻿using System.Collections.Immutable;
using PostToys.Common;
using PostToys.Parse.Model;
using PostToys.Post.Model;
using Spectre.Console;
using Spectre.Console.Json;
using Spectre.Console.Rendering;

namespace PostToys;

/// <summary>
/// 打印输出响应信息
/// </summary>
public static class PrintBoy
{
    /// <summary>
    /// 打印响应信息：<see cref="Boy"/>
    /// </summary>
    /// <param name="boy"></param>
    public static void Print(this IBoy boy)
    {
        var toy = boy.Toy;
        if (toy == new Toy())
        {
            PrintError($"{boy.ReasonPhrase}");
            return;
        }

        PrintSeparator($"{toy.Name.Trim()} request start");

        switch (boy)
        {
            case HttpBoy httpBoy:
                PrintHttp(httpBoy);
                break;
            case DatabaseBoy databaseBoy:
                PrintDatabase(databaseBoy);
                break;
        }

        PrintSeparator($"{toy.Name.Trim()} request end");
        Console.Beep();
    }

    private static void PrintRequestBody(Toy toy)
    {
        if (toy.Body is { Length: > 0 })
        {
            Print(toy.Body, beforeNewLine: true);
        }
    }

    private static void PrintHttp(HttpBoy boy)
    {
        PrintMethod(boy.Method);
        Print($" {boy.Url} {boy.Version} ");

        if (boy.RequestHeader is { Count: > 0 })
        {
            foreach (var (key, value) in boy.RequestHeader)
            {
                Print($"{key}: {value}");
            }
        }

        PrintRequestBody(boy.Toy);

        Print($"{boy.Url} {boy.Version} ", true, false);

        var state = $"{Convert.ToInt32(boy.StatusCode)} [{boy.ReasonPhrase}]";
        PrintMessage(state, boy.IsSuccessStatusCode, afterNewLine: false);

        PrintFormatText($" {boy.TakeTime}ms", foreground: Color.DarkMagenta);

        if (boy.ResponseHeader is not { Count: > 0 }) return;

        foreach (var (key, value) in boy.ResponseHeader)
        {
            Print($"{key}: {value.Aggregate((x1, x2) => $"{x1}; {x2}")}");
        }
        
        if (boy.Body is { Length: > 0 })
        {
            Print(boy.Body, beforeNewLine: true);
        }
    }

    private static void PrintDatabase(DatabaseBoy boy)
    {
        PrintMethod(boy.Method);
        Print($" {boy.Url} {boy.DatabaseType} ", false, false);
        PrintMessage("SUCCESS", boy.IsSuccessStatusCode, afterNewLine: false);

        PrintFormatText($" {boy.TakeTime}ms", foreground: Color.DarkMagenta);

        PrintRequestBody(boy.Toy);
        
        PrintJsonToTable(boy.Body);
    }

    private static void PrintJsonToTable(string json)
    {
        if (!JsonUtil.IsJson(json))
        {
            return;
        }

        var list = JsonUtil.FromJson<List<ImmutableSortedDictionary<string, object>>>(json);
        if (list == null && list is { Count: <= 0 })
        {
            return;
        }

        var table = new Table();
        table.Border(TableBorder.Heavy);
        
        table.AddColumns(list!.First().Keys.ToArray()).Centered();

        foreach (var array in list!.Select(dictionary => dictionary.Values.Select(it => it.ToString()).ToArray()))
        {
            table.AddRow(array!);
            table.AddEmptyRow();
        }

        AnsiConsole.Write(table);
    }

    /// <summary>
    /// 打印失败消息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="decoration">文本修饰<see cref="Decoration"/></param>
    /// <param name="beforeNewLine">是否前置换行</param>
    /// <param name="afterNewLine">是否后置换行</param>
    private static void PrintError(
        string message, Decoration decoration = Decoration.None,
        bool beforeNewLine = false, bool afterNewLine = true)
    {
        PrintFormatText(message, Color.DarkRed, Color.Red3_1, decoration, beforeNewLine, afterNewLine);
    }

    /// <summary>
    /// 打印成功消息
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="decoration">文本修饰<see cref="Decoration"/></param>
    /// <param name="beforeNewLine">是否前置换行</param>
    /// <param name="afterNewLine">是否后置换行</param>
    private static void PrintSuccess(
        string message, Decoration decoration = Decoration.None,
        bool beforeNewLine = false, bool afterNewLine = true)
    {
        PrintFormatText(message, Color.DarkGreen, Color.LightGreen, decoration, beforeNewLine, afterNewLine);
    }

    /// <summary>
    /// 打印消息，包括成功或失败
    /// </summary>
    /// <param name="message">消息内容</param>
    /// <param name="succeed">是否成功消息</param>
    /// <param name="decoration">文本修饰<see cref="Decoration"/></param>
    /// <param name="beforeNewLine">是否前置换行</param>
    /// <param name="afterNewLine">是否后置换行</param>
    private static void PrintMessage(
        string message, bool succeed, Decoration decoration = Decoration.None,
        bool beforeNewLine = false, bool afterNewLine = true)
    {
        if (succeed)
        {
            PrintSuccess(message, decoration, beforeNewLine, afterNewLine);
            return;
        }

        PrintError(message, decoration, beforeNewLine, afterNewLine);
    }

    /// <summary>
    /// 打印分隔符
    /// </summary>
    /// <param name="name">分隔符名称</param>
    private static void PrintSeparator(string name)
    {
        var sum = name.Sum(c => c > 127 ? 2 : 1);
        PrintFormatText($"#{name.ToUpper()} {new string('-', 50 - sum)}>", foreground: Color.Green);
    }

    /// <summary>
    /// 打印请求方法
    /// </summary>
    /// <param name="method">请求方法名</param>
    private static void PrintMethod(string method)
    {
        if (string.IsNullOrWhiteSpace(method))
        {
            method = string.Empty;
        }

        var (foreground, background) = method.ToUpper() switch
        {
            "GET" => (Color.DarkBlue, Color.LightSlateBlue),
            "POST" => (Color.DarkGreen, Color.LightGreen),
            "PUT" => (Color.DarkGoldenrod, Color.Yellow2),
            "DELETE" => (Color.DarkRed, Color.Red),
            _ => (Color.Default, Color.Grey30)
        };
        PrintFormatText(method, foreground, background, Decoration.Bold, false, false);
    }

    /// <summary>
    /// 打印格式文本
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="foreground">文本颜色</param>
    /// <param name="background">背景色</param>
    /// <param name="decoration">文本修饰</param>
    /// <param name="beforeNewLine">是否前置换行</param>
    /// <param name="afterNewLine">是否后置换行</param>
    private static void PrintFormatText(string text, Color? foreground = null, Color? background = null,
        Decoration decoration = Decoration.None, bool beforeNewLine = false, bool afterNewLine = true)
    {
        Print(new Text(text, new Style(foreground ?? Color.Default, background ?? Color.Default, decoration)),
            beforeNewLine, afterNewLine);
    }

    /// <summary>
    /// 打印输入内容
    /// </summary>
    /// <param name="value">需要打印的内容</param>
    /// <param name="beforeNewLine">是否前置换行</param>
    /// <param name="afterNewLine">是否后置换行</param>
    private static void Print(object? value = null, bool beforeNewLine = false, bool afterNewLine = true)
    {
        if (beforeNewLine)
        {
            Console.WriteLine();
        }

        if (value is string s && JsonUtil.IsJson(s))
        {
            value = new JsonText(s);
        }

        if (value is IRenderable r)
        {
            AnsiConsole.Write(r);
        }
        else
        {
            Console.Write(value);
        }

        if (afterNewLine)
        {
            Console.WriteLine();
        }
    }
}