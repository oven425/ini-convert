// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

Console.WriteLine("Hello, World!");
[Generator]
public class CustomGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static postInitializationContext => {
            postInitializationContext.AddSource("myGeneratedFile.cs", SourceText.From("""
                using System;

                namespace GeneratedNamespace
                {
                    internal sealed class GeneratedAttribute : Attribute
                    {
                    }
                }
                """, Encoding.UTF8));
        });
    }
}

