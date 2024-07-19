// See https://aka.ms/new-console-template for more information

using PostToys.Parse;
using PostToys.Parse.Markdown;

Console.WriteLine("Hello, World!");

AbstractParser parser =
    Parser.InputPath(
        @".\PostToys\PostToys.Parse.Markdown\test.md");
var toys = parser.Toys();
toys.ForEach(toy => Console.WriteLine(toy));