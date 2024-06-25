using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace QSoft.Ini
{
    [Generator]
    public class EnumGenerator : IIncrementalGenerator
    {
        private const string EnumExtensionsAttribute = "NetEscapades.EnumGenerators.EnumExtensionsAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            //context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            //    "EnumExtensionsAttribute.g.cs", SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));

            IncrementalValuesProvider<EnumDeclarationSyntax> enumDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                .Where(static m => m is not null)!;

            var classfegs = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetIniGenerate(ctx))
                .Where(static m => m is not null)!;

            IncrementalValueProvider<(Compilation, ImmutableArray<EnumDeclarationSyntax>)> compilationAndEnums
                = context.CompilationProvider.Combine(enumDeclarations.Collect());

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationAndClass
                = context.CompilationProvider.Combine(classfegs.Collect());

            //context.RegisterSourceOutput(compilationAndEnums,
            //    static (spc, source) => Execute(source.Item1, source.Item2, spc));

            context.RegisterSourceOutput(compilationAndClass,
                static (spc, source) => Execute1(source.Item1, source.Item2, spc));

        }

        static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        {
            bool csvdd = false;
            if( node is ClassDeclarationSyntax cc)
            {
                var a = cc.AttributeLists.SelectMany(x => x.Attributes)
                    .Select(x => x.Name.ToString());
                csvdd = a.Any(x=>x=="IniSection");


            }
            return csvdd;
        }

        static ClassDeclarationSyntax GetIniGenerate(GeneratorSyntaxContext context)
        {
            return (ClassDeclarationSyntax)context.Node;
        }


        static EnumDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
            var enumDeclarationSyntax = context.Node as EnumDeclarationSyntax;
            if (enumDeclarationSyntax == null) return null;

            // loop through all the attributes on the method
            foreach (AttributeListSyntax attributeListSyntax in enumDeclarationSyntax.AttributeLists)
            {
                foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                    {
                        // weird, we couldn't get the symbol, ignore it
                        continue;
                    }

                    INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                    string fullName = attributeContainingTypeSymbol.ToDisplayString();

                    // Is the attribute the [EnumExtensions] attribute?
                    if (fullName == EnumExtensionsAttribute)
                    {
                        // return the enum
                        return enumDeclarationSyntax;
                    }
                }
            }

            // we didn't find the attribute we were looking for
            return null;
        }

        static void Execute1(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> enums, SourceProductionContext context)
        {
            if (enums.IsDefaultOrEmpty)
            {
                // nothing to do yet
                return;
            }
            
            foreach (var enumDeclarationSyntax in enums)
            {
                
                var namespacestr = GetNamespace(enumDeclarationSyntax);

                SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
                {
                    // something went wrong
                    continue;
                }
                var classstr = enumSymbol.Name;
                var sss = @"
";
                var sb = new StringBuilder();
                sb.AppendLine($"namespace {namespacestr}");
                sb.AppendLine("{");
                sb.AppendLine($"\tpublic partial class {classstr}");
                sb.AppendLine("\t{");
                sb.AppendLine("\t\tpublic string GetInitString()");
                sb.AppendLine("\t\t{");

                sb.AppendLine("\t\t\tvar sb = new System.Text.StringBuilder();");
                sb.AppendLine($"\t\t\tsb.AppendLine($\"[{classstr}]\");");
                ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
                var members = new List<string>(enumMembers.Length);

                foreach (ISymbol member in enumMembers)
                {
                    if (member is IPropertySymbol field && !field.IsReadOnly)
                    {
                        members.Add(member.Name);
                        var mm = $"{member.Name}";
                        var mm1 = "{"+mm+"}";
                        sb.AppendLine($"\t\t\tsb.AppendLine($\"{member.Name}={mm1}\");");
                    }
                }
                //sb.AppendLine($"Name:{Name}");
                sb.AppendLine("\t\t\treturn sb.ToString();");
                sb.AppendLine("\t\t}");
                sb.AppendLine("\t}");
                sb.AppendLine("}");

                var svvv = sb.ToString();
                context.AddSource("EnumExtensions.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
            }
                
        }

        static string GetNamespace(BaseTypeDeclarationSyntax syntax)
        {
            // If we don't have a namespace at all we'll return an empty string
            // This accounts for the "default namespace" case
            string nameSpace = string.Empty;

            // Get the containing syntax node for the type declaration
            // (could be a nested type, for example)
            SyntaxNode? potentialNamespaceParent = syntax.Parent;

            // Keep moving "out" of nested classes etc until we get to a namespace
            // or until we run out of parents
            while (potentialNamespaceParent != null &&
                    potentialNamespaceParent is not NamespaceDeclarationSyntax
                    && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
            {
                potentialNamespaceParent = potentialNamespaceParent.Parent;
            }

            // Build up the final namespace by looping until we no longer have a namespace declaration
            if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
            {
                // We have a namespace. Use that as the type
                nameSpace = namespaceParent.Name.ToString();

                // Keep moving "out" of the namespace declarations until we 
                // run out of nested namespace declarations
                while (true)
                {
                    if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                    {
                        break;
                    }

                    // Add the outer namespace as a prefix to the final namespace
                    nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                    namespaceParent = parent;
                }
            }

            // return the final namespace
            return nameSpace;
        }


        static void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax> enums, SourceProductionContext context)
        {
            if (enums.IsDefaultOrEmpty)
            {
                // nothing to do yet
                return;
            }

            // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
            IEnumerable<EnumDeclarationSyntax> distinctEnums = enums.Distinct();

            // Convert each EnumDeclarationSyntax to an EnumToGenerate
            List<EnumToGenerate> enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

            // If there were errors in the EnumDeclarationSyntax, we won't create an
            // EnumToGenerate for it, so make sure we have something to generate
            //if (enumsToGenerate.Count > 0)
            //{
            //    // generate the source code and add it to the output
            //    string result = SourceGenerationHelper.GenerateExtensionClass(enumsToGenerate);
            //    context.AddSource("EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
            //}
        }

        static List<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax> enums, CancellationToken ct)
        {
            var enumsToGenerate = new List<EnumToGenerate>();
            INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName(EnumExtensionsAttribute);
            if (enumAttribute == null)
            {
                // nothing to do if this type isn't available
                return enumsToGenerate;
            }

            foreach (var enumDeclarationSyntax in enums)
            {
                // stop if we're asked to
                ct.ThrowIfCancellationRequested();

                SemanticModel semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
                {
                    // something went wrong
                    continue;
                }

                string enumName = enumSymbol.ToString();
                string extensionName = "EnumExtensions";

                foreach (AttributeData attributeData in enumSymbol.GetAttributes())
                {
                    if (!enumAttribute.Equals(attributeData.AttributeClass, SymbolEqualityComparer.Default))
                    {
                        continue;
                    }

                    foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
                    {
                        if (namedArgument.Key == "ExtensionClassName"
                            && namedArgument.Value.Value?.ToString() is { } n)
                        {
                            extensionName = n;
                        }
                    }
                    break;
                }

                ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
                var members = new List<string>(enumMembers.Length);

                foreach (ISymbol member in enumMembers)
                {
                    if (member is IFieldSymbol field && field.ConstantValue is not null)
                    {
                        members.Add(member.Name);
                    }
                }

                enumsToGenerate.Add(new EnumToGenerate(extensionName, enumName, members));
            }

            return enumsToGenerate;
        }
    }


    public readonly struct EnumToGenerate
    {
        public readonly string ExtensionName;
        public readonly string Name;
        public readonly List<string> Values;

        public EnumToGenerate(string extensionName, string name, List<string> values)
        {
            Name = name;
            Values = values;
            ExtensionName = extensionName;
        }
    }
}
