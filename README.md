# PostToys

PostToys 是一个用于发送 HTTP 请求并处理响应的应用程序。它提供了一个简单的接口来执行 HTTP 请求，并返回详细的响应信息。

## 特性

- 支持多种 HTTP 版本（如 HTTP/1.0, HTTP/1.1, HTTP/2.0, HTTP/3.0）
- 简单易用的 API
- 支持自定义请求头和请求体
- 详细的响应信息，包括状态码、响应头、响应体等
- 默认支持 Markdown 的请求信息配置，同时还可以拓展其他的请求配置文件
- 后续将添加与数据库操作相关的功能，可以联合 HTTP 请求一起使用

## 使用到的框架和库

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Spectre.Console 0.49.1](https://spectreconsole.net/)
- [Spectre.Console.Json 0.49.1](https://spectreconsole.net/)

## 安装

请确保您已安装 .NET SDK。然后可以克隆此仓库并运行应用程序：

```bash
git clone https://github.com/yourusername/PostToys.git
cd PostToys
dotnet run
```

## 功能描述

PostToys 主要功能包括：

1. **发送 HTTP 请求**：支持 GET、POST 等多种 HTTP 方法，支持自定义请求头和请求体。
2. **处理 HTTP 响应**：返回详细的响应信息，包括状态码、响应头、响应体等。
3. **HTTP 版本支持**：支持 HTTP/1.0, HTTP/1.1, HTTP/2.0, HTTP/3.0 等多种 HTTP 版本。
4. **默认支持 Markdown 的请求信息配置**：可以通过 Markdown 文件配置请求信息，同时还可以拓展其他的请求配置文件格式。
5. **后续功能**：将添加与数据库操作相关的功能，可以联合 HTTP 请求一起使用，提供更强大的数据处理能力。

## 使用示例

以下是如何使用 PostToys 通过 `Runner.Run` 方法来执行基于 Markdown 文件的接口请求配置：

```csharp
// 传入 Markdown 请求配置文件和接口名
Runner.Run("test.md", "add");
```

## Markdown 请求配置文件示例

Markdown 示例文件见 [test.md](PostToys.Parse.Markdown/test.md)，用于配置接口请求。


## 贡献

欢迎任何形式的贡献！请提交问题或拉取请求。

## 许可证

本项目采用 MIT 许可证，详细信息请参见 [LICENSE](LICENSE) 文件。
