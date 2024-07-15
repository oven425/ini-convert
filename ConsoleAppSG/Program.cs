// See https://aka.ms/new-console-template for more information
using System.Net;

//https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11#raw-string-literals

Console.WriteLine("Hello, World!");
string longMessage = """
    This is a long message.
    It has several lines.
        Some are indented
                more than others.
    Some should start at the first column.
    Some have "quoted text" in them.
    """;
var Longitude = 100;
var Latitude = 200;
var location = $$"""
   You are at {{{Longitude}}, {{Latitude}}}
   """;

Console.ReadLine();