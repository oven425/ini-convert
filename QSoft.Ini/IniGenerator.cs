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
        const string m_Section = " QSoft.Ini.IniSectionAttribute";
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
                   var cls = ctx.Node as ClassDeclarationSyntax;
                   var a = cls.AttributeLists.SelectMany(x => x.Attributes)
                    .Select(x => x.Name);

                   if (a.Any(x => x.ToString() == "IniSection"))
                       return cls;
                   return null;
               }).Where(static m => m != null);

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClass
                = context.CompilationProvider.Combine(classfegs.Collect());

            context.RegisterSourceOutput(compilationAndClass,
                static (spc, source) =>
                {
                    foreach(var cls in source.Item2)
                    {

                    }
                });
        }
    }



    
}
