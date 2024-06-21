using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

//https://www.cnblogs.com/wanghun315/p/17236982.html
namespace QSoft.Ini
{
    [Generator]
    public class ToStringGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            return;
            AugmentSyntaxReceiver syntaxReceiver = (AugmentSyntaxReceiver)context.SyntaxReceiver;

            // 通过语法接收器获得class类
            ClassDeclarationSyntax augmentClass = syntaxReceiver.ClassToAugment;
            //判断是否有这个类
            if (augmentClass is null)
            {
                //没有找到就不做任何事情
                return;
            }

            //找到了就添加一个方法
            SourceText sourceText = SourceText.From($@"
namespace {syntaxReceiver.SpaceToAugment.Name.GetText()}
{{
    public partial class {augmentClass.Identifier}
    {{
        public void GeneratedMethod()
        {{
            Console.WriteLine(""Hello, augmentClass!"");
            Console.WriteLine(""{ DateTime.Now}"");
        }}
    }}
}}", Encoding.UTF8);
            context.AddSource("augmentClass.Generated.cs", sourceText);

            //            context.AddSource("Example.g.cs",@"
            //public static class Example {
            //    	public static int Value = 42;
            //}");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }
#endif

            //context.RegisterForSyntaxNotifications(() => new AugmentSyntaxReceiver());
            context.RegisterForSyntaxNotifications(() => new CsvSyntaxReceiver());
        }
    }

    public class CsvSyntaxReceiver : ISyntaxReceiver
    {
        public ClassDeclarationSyntax ClassToAugment { get; private set; }
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            System.Diagnostics.Trace.WriteLine($"{syntaxNode.GetType().Name}");
            if (syntaxNode is ClassDeclarationSyntax cds)
            {
                var s1s = cds.AttributeLists.SelectMany(x => x.Attributes);
                
                foreach (var oo in cds.AttributeLists)
                {
                    //syntaxNode.get
                    var ss = oo.SyntaxTree;
                }


            }
        }
    }

    class AugmentSyntaxReceiver : ISyntaxReceiver
    {
        // 获得类名
        public ClassDeclarationSyntax ClassToAugment { get; private set; }
        // 获得命名空间
        public NamespaceDeclarationSyntax SpaceToAugment { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            //Debugger.Launch();
            if (syntaxNode is NamespaceDeclarationSyntax csd)
            {
                SpaceToAugment = csd;
            }
            // 判断是否有AugmentClass，如果有则赋值
            if (syntaxNode is ClassDeclarationSyntax cds && cds.Identifier.ValueText == "AugmentClass")
            {

                ClassToAugment = cds;
            }
        }
    }
}
