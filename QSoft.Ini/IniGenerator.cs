using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace QSoft.Ini
{
    //public class IniGenerator : IIncrementalGenerator
    //{
    //    public void Initialize(IncrementalGeneratorInitializationContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    [AttributeUsage(AttributeTargets.Class)]
    public class IniSectionAttribute:Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IniIgnoreAttribute : Attribute
    {
    }
}
