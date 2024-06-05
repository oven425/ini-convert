using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace QSoft.Ini
{
    [Generator]
    public class ToStringGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource("Example.g.cs",@"
public static class Example {
    	public static int Value = 42;
}");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//            {
//                Debugger.Launch();
//            }
//#endif 
            //Debug.WriteLine("Initalize code generator");
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
