using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QSoft.Ini
{
    [DisplayName("IniSection")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false)]
    public class IniSectionAttribute : Attribute
    {
        public string Name { set; get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class IniIgnoreAttribute : Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class IniAnnotation : Attribute
    {
        public string Annotation { set; get; }
    }
}
