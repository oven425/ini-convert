﻿using Microsoft.CodeAnalysis;
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
                //Debugger.Launch();
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
                        SemanticModel semanticModel = source.Item1.GetSemanticModel(cls.SyntaxTree);
                        if (semanticModel.GetDeclaredSymbol(cls) is not INamedTypeSymbol symbol)
                        {
                            // something went wrong
                            continue;
                        }
                        var classname = cls.Identifier.ToString();
                        var pps = symbol.GetMembers()
                            .Where(x=>x is IPropertySymbol)
                            .Select(x=>(IPropertySymbol)x)
                            .Where(x=>!x.IsReadOnly);
                        foreach(var pp in pps)
                        {
                            var tt = pp.Type;
                            var aa = $"{pp.Name}={pp.Name}";
                            //writer.WriteString
                        }

                        var code1 = Enumerable.Range(1, 4).Select(x => $"var bb{x}={x};").Aggregate("", (x, y) => $"{x}{y}\r\n");

                        var code = @$"
using System.Text;

namespace QSoft.Ini
{{
    internal partial class {classname}
    {{
        internal void Serialize(IniWriter writer)
        {{
        "
        +
@$"         {code1}"
            


        +
        
        
@$"     }}
    }}

}}";
                        spc.AddSource($"{classname}.g.cs", code);
                    }
                });
        }
    }



    
}
