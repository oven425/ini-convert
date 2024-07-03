using System;
using System.Collections.Generic;
using System.Text;

namespace QSoft.Ini
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IniSectionAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IniIgnoreAttribute : Attribute
    {
    }
}
