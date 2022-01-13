using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace QSoft.Ini
{
    internal class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern Boolean WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }

    public class IniSerializer
    {
        Dictionary<string, IniDictionary> m_Sections = new Dictionary<string, IniDictionary>();
        public void Serialize(string section, Dictionary<string, object> datas, string filename)
        {
            FileInfo fileinfo = new FileInfo(filename);
            for (int i=0; i< datas.Count; i++)
            {
                object oo = datas.ElementAt(i).Value;
                if(oo==null)
                {
                    continue;
                }
                Type type = oo.GetType();
                
                TypeCode code = Type.GetTypeCode(type);
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.String:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Single:
                    case TypeCode.Char:
                    case TypeCode.Boolean:
                    case TypeCode.DateTime:
                        {
                            this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            if(type.BaseType != null)
                            {
                                this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
                            }
                        }
                        break;
                }
            }
        }

        



        void WriteINI(string section, string key, object oo, string  filename)
        {
            if(oo != null)
            {
                Type type = oo.GetType();
                TypeCode code = Type.GetTypeCode(type);
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Double:
                        {
                            NativeMethods.WritePrivateProfileString(section, key, ((double)oo).ToString("r"), filename);
                        }
                        break;
                    case TypeCode.String:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Decimal:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Single:
                    case TypeCode.Char:
                    case TypeCode.Boolean:
                        {
                            NativeMethods.WritePrivateProfileString(section, key, oo.ToString(), filename);
                        }
                        break;
                    case TypeCode.DateTime:
                        {
                            DateTime datetime = (DateTime)oo;
                            NativeMethods.WritePrivateProfileString(section, key, datetime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo), filename);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            if (oo is TimeSpan)
                            {
                                NativeMethods.WritePrivateProfileString(section, key, ((TimeSpan)oo).ToString(), filename);
                            }
                            else
                            {
                                
                                XmlSerializer xml = new XmlSerializer(type);
                                using (MemoryStream mm = new MemoryStream())
                                {
                                    using (StreamWriter sw = new StreamWriter(mm, Encoding.UTF8))
                                    {
                                        using (var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8 }))
                                        {
                                            xml.Serialize(xmlWriter, oo);
                                        }
                                        byte[] bb = mm.ToArray();

                                        //because utf-8 begin is 0xef 0xbb 0xbf
                                        //remove 3 byte, WritePrivateProfileStringW is no ? at being
                                        string str = Encoding.UTF8.GetString(bb, 3, bb.Length - 3);
                                        NativeMethods.WritePrivateProfileString(section, key, str, filename);
                                    }
                                }
                            }

                        }
                        break;
                }
            }
            else
            {
                NativeMethods.WritePrivateProfileString(section, key, "", filename);
            }
        }

        public void Deserialize(string section, Dictionary<string, object> datas, string filename)
        {
            FileInfo fileinfo = new FileInfo(filename);
            for (int i = 0; i < datas.Count; i++)
            {
                Type type = datas.ElementAt(i).Value?.GetType();
                if(type == null)
                {
                    continue;
                }
                object oo = datas.ElementAt(i).Value;
                TypeCode code = Type.GetTypeCode(type);
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.String:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        {
                            datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, type, code);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            //if(type.IsArray == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            //}
                            //else if(type.IsClass == true&& type.IsGenericType == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            //}
                            //else if(type.BaseType == null)
                            //{

                            //}
                            //else if(oo is TimeSpan)
                            //{
                            //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            //}
                            //else
                            //{
                            //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            //}

                            if (type.BaseType != null)
                            {
                                datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, type, code);
                            }
                        }
                        break;
                }
            }
        }

        public object ReadIni(string section, string key, object oo, string filename, Type type, TypeCode typecode)
        {
            object dst = null;
            
            StringBuilder temp = new StringBuilder(1024);
            
            NativeMethods.GetPrivateProfileString(section, key, "", temp, 255, filename);
            if(temp.Length <=0)
            {
                return dst;
            }
            switch (typecode)
            {
                case TypeCode.Decimal:
                    {
                        Decimal a = 0;
                        Decimal.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Boolean:
                    {
                        bool a = false;
                        bool.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Byte:
                    {
                        Byte a = 0;
                        Byte.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Char:
                    {
                        Char a = Char.MinValue;
                        Char.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.SByte:
                    {
                        sbyte a = 0;
                        sbyte.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Single:
                    {
                        float a = 0;
                        float.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Double:
                    {
                        double a = 0;
                        double.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.String:
                    {
                        dst = temp.ToString();
                    }
                    break;
                case TypeCode.Int16:
                    {
                        Int16 a = 0;
                        Int16.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Int32:
                    {
                        Int32 a = 0;
                        Int32.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Int64:
                    {
                        Int64 a = 0;
                        Int64.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.UInt16:
                    {
                        UInt16 a = 0;
                        UInt16.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.UInt32:
                    {
                        UInt32 a = 0;
                        UInt32.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.UInt64:
                    {
                        UInt64 a = 0;
                        UInt64.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.DateTime:
                    {
                        DateTime a = DateTime.MinValue;
                        DateTime.TryParse(temp.ToString(), out a);
                        dst = a;
                    }
                    break;
                case TypeCode.Object:
                    {
                        if(oo is TimeSpan)
                        {
                            TimeSpan a = new TimeSpan();
                            TimeSpan.TryParse(temp.ToString(), out a);
                            dst = a;
                        }
                        else
                        {
                            string ss = temp.ToString();
                            if (string.IsNullOrEmpty(ss) == false && ss.Trim().Length > 0)
                            {
                                XmlSerializer xml = new XmlSerializer(type);
                                Encoding.UTF8.GetBytes(ss);
                                using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(ss)))
                                {
                                    dst = xml.Deserialize(mm);
                                }
                            }
                        }
                    }
                    break;
            }
            return dst;
        }

        public string Serialize(object obj)
        {
            //FileInfo file = new FileInfo(filename);
            Type type = obj.GetType();
            TypeCode typecode = Type.GetTypeCode(type);
            if (typecode != TypeCode.Object)
            {
                return "";
            }
            StringBuilder strb = new StringBuilder();
            IniSection defaultsection = type.GetCustomAttributes(typeof(IniSection), false).FirstOrDefault() as IniSection;
            strb.AppendLine($"[{type.Name}]");

            var pps = type.GetProperties().Where(x => x.CanWrite && x.CanRead);
            foreach (PropertyInfo pp in pps)
            {
                var attrs = pp.GetCustomAttributes(true);
                var attribe = pp.GetCustomAttributes(typeof(IniSectionKey), false).FirstOrDefault() as IniSectionKey;
                string section = type.Name;
                if (defaultsection != null && string.IsNullOrEmpty(defaultsection.DefaultSection) == true && defaultsection.DefaultSection.Trim().Length > 0)
                {
                    section = defaultsection.DefaultSection;
                }
                string key = pp.Name;
                bool ignore = attrs.Any(x => x is IniIgnore);
                if (attribe != null)
                {
                    if (string.IsNullOrEmpty(attribe.Section) == false && attribe.Section.Trim().Length > 0)
                    {
                        section = attribe.Section;
                    }
                    if (string.IsNullOrEmpty(attribe.Key) == false && attribe.Key.Trim().Length > 0)
                    {
                        key = attribe.Key;
                    }
                    //ignore = attribe.Ignore;
                }
                if (ignore == false)
                {
                    strb.AppendLine($"{key}={pp.GetValue(obj, null)}");
                    //this.WriteINI(section, key, pp.GetValue(obj, null), file.FullName);
                }
            }
            return strb.ToString();
        }


        void Serialize(PropertyInfo property, object obj)
        {

        }




        public void Serialize(object obj, string filename)
        {
            FileInfo file = new FileInfo(filename);
            Type type = obj.GetType();
            TypeCode typecode = Type.GetTypeCode(type);
            if(typecode != TypeCode.Object)
            {
                return;
            }
            
            IniSection defaultsection = type.GetCustomAttributes(typeof(IniSection), false).FirstOrDefault() as IniSection;
            

            var pps = type.GetProperties().Where(x => x.CanWrite && x.CanRead);
            foreach (PropertyInfo pp in pps)
            {
                var attrs = pp.GetCustomAttributes(true);
                var attribe = pp.GetCustomAttributes(typeof(IniSectionKey), false).FirstOrDefault() as IniSectionKey;
                string section = type.Name;
                if (defaultsection != null && string.IsNullOrEmpty(defaultsection.DefaultSection) == true && defaultsection.DefaultSection.Trim().Length > 0)
                {
                    section = defaultsection.DefaultSection;
                }
                string key = pp.Name;
                bool ignore = attrs.Any(x=>x is IniIgnore);
                if (attribe != null)
                {
                    if (string.IsNullOrEmpty(attribe.Section) == false&& attribe.Section.Trim().Length > 0)
                    {
                        section = attribe.Section;
                    }
                    if (string.IsNullOrEmpty(attribe.Key) == false && attribe.Key.Trim().Length > 0)
                    {
                        key = attribe.Key;
                    }
                    //ignore = attribe.Ignore;
                }
                if (ignore == false)
                {
                    this.WriteINI(section, key, pp.GetValue(obj, null), file.FullName);
                }
            }
        }

        public void Deserialize(object obj, string filename)
        {
            FileInfo file = new FileInfo(filename);
            Type type = obj.GetType();
            TypeCode typecode = Type.GetTypeCode(type);
            if (typecode != TypeCode.Object)
            {
                return;
            }
            IniSection defaultsection = type.GetCustomAttributes(typeof(IniSection), false).FirstOrDefault() as IniSection;
            
            var pps = type.GetProperties().Where(x=>x.CanWrite&&x.CanRead);
            foreach (PropertyInfo pp in pps)
            {
                var attrs = pp.GetCustomAttributes(true);
                var attribe = pp.GetCustomAttributes(typeof(IniSectionKey), false).FirstOrDefault() as IniSectionKey;
                string section = type.Name;
                if (defaultsection != null && string.IsNullOrEmpty(defaultsection.DefaultSection) == true && defaultsection.DefaultSection.Trim().Length > 0)
                {
                    section = defaultsection.DefaultSection;
                }
                string key = pp.Name;
                bool ignore = attrs.Any(x => x is IniIgnore);
                if (attribe != null)
                {
                    if (string.IsNullOrEmpty(attribe.Section) == false && attribe.Section.Trim().Length>0)
                    {
                        section = attribe.Section;
                    }
                    if (string.IsNullOrEmpty(attribe.Key) == false&& attribe.Key.Trim().Length>0)
                    {
                        key = attribe.Key;
                    }
                    //ignore = attribe.Ignore;
                }
                if (ignore == false)
                {
                    object src = pp.GetValue(obj, null);
                    object dst = this.ReadIni(section, key, src, file.FullName, pp.PropertyType, Type.GetTypeCode(pp.PropertyType));
                    pp.SetValue(obj, dst, null);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class IniSection : Attribute
    {
        public string DefaultSection { set; get; }
    }


    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IniSectionKey:Attribute
    {
        public string Section { set; get; }
        public string Key { set; get; }
        //public bool Ignore { set; get; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IniIgnore:Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IniComment : Attribute
    {
        public IniComment(string comment)
        {
            this.Comment = comment;
        }
        public string Comment { set; get; }
    }
    //comment

    //[AttributeUsage(AttributeTargets.Property, Inherited = false)]
    //public class IniArray:Attribute
    //{
    //    public string Name { set; get; }
    //}

    //[AttributeUsage(AttributeTargets.Property, Inherited = false)]
    //public class IniArrayItem : Attribute
    //{
    //    public IniArrayItem(string name)
    //    {
    //        this.Name = name;
    //    }
    //    public string Name { private set; get; }
    //}

    public class IniDictionary : Dictionary<string, string>
    {
        public string Name { set; get; }
    }
}
