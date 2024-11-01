// See https://aka.ms/new-console-template for more information

using PostToys;

Console.WriteLine("Hello, World!");

Runner.Run(
    @"\PostToys.Parse.Markdown\test.md",
    @"\PostToys.Parse.Markdown\env.json", "local",
    "接口文档@1 新增"
    );
