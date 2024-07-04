using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace QSoft.Ini
{
    [Generator]
    public class IniGenerator : IIncrementalGenerator
    {
        const string m_IniSectionFullName = "QSoft.Ini.IniSectionAttribute";
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif
            var classfegs = context.SyntaxProvider
               .CreateSyntaxProvider(static (node, _) =>
               {
                   return node is ClassDeclarationSyntax;
               }, 
               static (ctx, _) =>
               {
                   var clssyntax = ctx.Node as ClassDeclarationSyntax;
                   if (clssyntax == null) return null;
                   foreach (AttributeListSyntax attributeListSyntax in clssyntax.AttributeLists)
                   {
                       foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                       {
                           if (ctx.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                           {
                               // weird, we couldn't get the symbol, ignore it
                               continue;
                           }
                           INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                           string fullName = attributeContainingTypeSymbol.ToDisplayString();

                           // Is the attribute the [EnumExtensions] attribute?
                           if (fullName == m_IniSectionFullName)
                           {
                               // return the enum
                               return clssyntax;
                           }

                       }
                   }
                   return null;
               }).Where(static m => m != null);

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClass
                = context.CompilationProvider.Combine(classfegs.Collect());

            context.RegisterSourceOutput(compilationAndClass,
                static (spc, source) =>
                {
                    var classstntaxs = source.Item2.Distinct();
                    foreach(var cls in classstntaxs)
                    {
                        var clasllname = cls.Identifier.ToString();
                        //var pps = cls.Members.Select(X => X.SyntaxTree.);
                        var aa = source.Item1.GetSemanticModel(cls.SyntaxTree).GetSymbolInfo(cls).Symbol;
                        var code = @"
using System.Text;

namespace QSoft.Ini
{
    internal partial class "+$"{clasllname}"+@"
    {
        internal void Serialize(IniWriter writer)
        {
            "+@"
        }
    }

}";
                        spc.AddSource($"{clasllname}.g.cs", code);
                    }
                });
        }
    }



    
}
