using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.ComponentModel;

namespace QSoft.Ini
{
    //[DisplayName("IniConvert")]
    //public class IniConvertAttribute: Attribute
    //{
    //    public IniConvertAttribute(Type convert)
    //    {

    //    }
    //}

    

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IniSectionKey : Attribute
    {
        [Obsolete("Please use IniConvert")]
        public string Section { set; get; }
        public string Key { set; get; }
        [Obsolete("Please use IniIgnore")]
        public bool Ignore { set; get; }
    }

    //[DisplayName("IniIgnore")]
    //[AttributeUsage(AttributeTargets.Property, Inherited = false)]
    //public class IniIgnoreAttribute : Attribute
    //{
    //    public bool IsVisible { set; get; }
    //}

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class IniArray : Attribute
    {
        public int BaseIndex { set; get; }
        public string Name { set; get; }
    }

    

    public class ListDictonary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        List<TKey> m_Keys = new List<TKey>();
        public new void Add(TKey key, TValue value)
        {
            this.CheckKey(key);
            base.Add(key, value);
        }

        public new void Clear()
        {
            m_Keys.Clear();
            base.Clear();
        }

        public new TValue this[TKey key]
        {
            set
            {
                this.CheckKey(key);
                base[key] = value;
            }
            get
            {
                return base[key];
            }
        }

        public new IEnumerable<TValue> Values
        {
            get
            {
                foreach(var oo in this.m_Keys)
                {
                    yield return this[oo];
                }
            }
        }

        void CheckKey(TKey key)
        {
            if (this.ContainsKey(key) == false)
            {
                this.m_Keys.Add(key);
            }
        }

    }

    //interface IniString
    //{
    //    string WriteToString();
    //}

    //public abstract class IniConvert<T>
    //{
    //    public abstract string ConvertTo(T obj);
    //    public abstract T ConvertTo(string data);
    //}

    public static class IniConvert
    {
        static string GetSectionName(this Type type)
        {
            var section = type.GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionAttribute) as IniSectionAttribute;
            return section?.Name ?? type.Name;
        }

        static string GetSectionKeyName(this PropertyInfo type)
        {
            var section = type.GetCustomAttributes(true).FirstOrDefault(x => x is IniSectionKey) as IniSectionKey;
            return section?.Key ?? type.Name;
        }

        static public string SerializeObject(object obj)
        {
            Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
            List<string> sectionlist = new List<string>();
            Dictionary<string, Dictionary<string, string>> comments = new Dictionary<string, Dictionary<string, string>>();
            Pack(obj, obj.GetType().GetSectionName(), sections, sectionlist, comments);

            StringBuilder strb = new StringBuilder();
            foreach (var section in sectionlist)
            {
                strb.AppendLine($"[{section}]");
                
                foreach (var oo in sections[section])
                {
                    var comment = comments[section][oo.Key];
                    if(string.IsNullOrEmpty(comment)==false)
                    {
                        strb.Append($"{comment}");
                    }
                    
                    strb.AppendLine($"{oo.Key}={oo.Value}");
                }
                strb.AppendLine();
            }
            return strb.ToString();
        }

        static string ToXml(object obj)
        {
            string str = "";
            XmlSerializer xml = new XmlSerializer(obj.GetType());
            using (MemoryStream mm = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(mm, Encoding.UTF8))
                {
                    using (var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8 }))
                    {
                        xml.Serialize(xmlWriter, obj);
                    }
                    byte[] bb = mm.ToArray();
                    str = Encoding.UTF8.GetString(bb, 3, bb.Length - 3);
                    //because utf-8 begin is 0xef 0xbb 0xbf
                    //remove 3 byte, WritePrivateProfileStringW is no ? at being
                    //string str = Encoding.UTF8.GetString(bb, 3, bb.Length - 3);
                    //NativeMethods.WritePrivateProfileString(section, key, str, filename);
                }
            }

            return str;
        }



        static void Pack(object obj, string section_name, Dictionary<string, Dictionary<string, string>> sections, List<string> sectionlist, Dictionary<string, Dictionary<string, string>> comments)
        {
            if (sections.ContainsKey(section_name) == false)
            {
                sectionlist.Add(section_name);
                sections[section_name] = new Dictionary<string, string>();
                comments[section_name] = new Dictionary<string, string>();
            }
            var pps = obj.GetType().GetProperties()
                .Select(x => new { attrs = x.GetCustomAttributes(true), property = x, typecode = Type.GetTypeCode(x.PropertyType) })
                .Where(x => x.property.CanRead == true && x.attrs.Any(y => y is IniIgnoreAttribute) == false);
            foreach (var pp in pps)
            {
                switch (pp.typecode)
                {
                    case TypeCode.Object:
                        {
                            if (pp.property.PropertyType.IsGenericType == true)
                            {
                                var define = pp.property.PropertyType.GetGenericTypeDefinition();
                                if (define == typeof(IEnumerable<>) || define == typeof(List<>))
                                {
                                    var ienumable = pp.property.GetValue(obj, null) as IEnumerable<object>;
                                    if(ienumable == null)
                                    {
                                        continue;
                                    }
                                    var iniarray = pp.attrs.FirstOrDefault(x => x is IniArray) as IniArray;
                                    if (iniarray == null)
                                    {
                                        string xml = ToXml(pp.property.GetValue(obj, null));
                                        sections[section_name][pp.property.Name] = xml;
                                    }
                                    else
                                    {
                                        int index = iniarray.BaseIndex;
                                        foreach (var oo in ienumable)
                                        {
                                            Pack(oo, $"{iniarray.Name??pp.property.Name}_{index++}", sections, sectionlist, comments);
                                        }
                                    }
                                }
                                else
                                {
                                    string xml = ToXml(pp.property.GetValue(obj, null));
                                    sections[section_name][pp.property.Name] = xml;
                                }
                            }
                            else if (pp.property.PropertyType == typeof(TimeSpan))
                            {
                                sections[section_name][pp.property.Name] = $"{pp.property.GetValue(obj, null)}";
                            }
                            else
                            {
                                var subobj = pp.property.GetValue(obj, null);
                                if (subobj != null)
                                {
                                    var subobj_section = pp.attrs.FirstOrDefault(x => x is IniSectionAttribute) as IniSectionAttribute;
                                    if (subobj_section != null)
                                    {
                                        string sub_name = pp.property.Name;
                                        if(string.IsNullOrEmpty(subobj_section.Name) == false)
                                        {
                                            sub_name = subobj_section.Name;
                                        }
                                        Pack(subobj, sub_name, sections, sectionlist, comments);
                                    }
                                    else
                                    {
                                        string xml = ToXml(subobj);
                                        sections[section_name][pp.property.Name] = xml;
                                    }
                                }
                            }
                        }
                        break;
                    case TypeCode.Double:
                        {
                            var vv = (double)pp.property.GetValue(obj, null);
                            sections[section_name][pp.property.Name] = $"{vv.ToString("R")}";
                        }
                        break;
                    default:
                        {
                            var subobj = pp.property.GetValue(obj, null);
                            if(subobj != null)
                            {
                                string comment = "";
                                var comms = pp.attrs.Where(x => x is IniAnnotation);
                                foreach (var oo in comms)
                                {
                                    var comm = oo as IniAnnotation;
                                    comment = $"{comment};{comm.Annotation}\r\n";
                                }
                                comments[section_name][pp.property.GetSectionKeyName()] = comment;
                                sections[section_name][pp.property.GetSectionKeyName()] = $"{pp.property.GetValue(obj, null)}";
                            }
                        }
                        break;
                }
            }
        }

        static public T DeserializeObject<T>(string ini) where T : class
        {
            var dics = Parse(ini);
            var type = typeof(T);
            var oi = type.GetCustomAttributes(true).FirstOrDefault(x => x == typeof(IniSectionAttribute));
            var section_name = type.GetSectionName();
            if (dics.ContainsKey(section_name) == true)
            {
                var obj = Activator.CreateInstance<T>();
                UnPack(dics, obj, section_name);
                return obj;
            }


            return default(T);
        }

        static object FromXml(string str , Type type)
        {
            object obj = null;
            XmlSerializer xml = new XmlSerializer(type);
            using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                obj = xml.Deserialize(mm);
            }
            
            return obj;
        }

        static void UnPack(Dictionary<string, Dictionary<string, string>> ini, object obj, string section_name)
        {
            var pps = obj.GetType().GetProperties().Where(x => x.CanWrite == true)
                .Select(x => new { x, property = x.PropertyType, attrs = x.GetCustomAttributes(true), typecode = Type.GetTypeCode(x.PropertyType) });
            pps = pps.Where(x => x.attrs.Any(y => y.GetType() == typeof(IniIgnoreAttribute)) == false);

            foreach (var pp in pps)
            {
                string str = null;
                if (ini[section_name].ContainsKey(pp.x.GetSectionKeyName()) == true)
                {
                    str = ini[section_name][pp.x.GetSectionKeyName()];
                }

                switch (pp.typecode)
                {
                    case TypeCode.Object:
                        {
                            if (pp.property.IsGenericType == true)
                            {
                                var define = pp.property.GetGenericTypeDefinition();
                                if (define == typeof(List<>))
                                {


                                    var ienumable = pp.x.GetValue(obj, null);
                                    if(ienumable == null)
                                    {
                                        ienumable = Activator.CreateInstance(define.MakeGenericType(pp.property.GetGenericArguments()[0]));
                                        pp.x.SetValue(obj, ienumable, null);
                                    }
                                    MethodInfo method = ienumable.GetType().GetMethod("Add");
                                    var iniarray = pp.attrs.FirstOrDefault(x => x is IniArray) as IniArray;
                                    if (iniarray == null)
                                    {
                                        var ddd = FromXml(str, pp.property);
                                        pp.x.SetValue(obj, ddd, null);
                                    }
                                    else
                                    {
                                        int index = iniarray.BaseIndex;
                                        while (true)
                                        {
                                            string subobj_section = $"{iniarray.Name??pp.x.Name}_{index++}";
                                            if (ini.ContainsKey(subobj_section) == true)
                                            {
                                                var subobj = Activator.CreateInstance(pp.property.GetGenericArguments()[0]);
                                                UnPack(ini, subobj, subobj_section);
                                                method.Invoke(ienumable, new object[] { subobj });
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                }
                            }
                            else if (pp.property == typeof(TimeSpan))
                            {
                                var timespan = TimeSpan.Parse(str);
                                pp.x.SetValue(obj, timespan, null);
                            }
                            else
                            {
                                var subobj_section = pp.attrs.FirstOrDefault(x => x is IniSectionAttribute) as IniSectionAttribute;
                                
                                if (subobj_section != null)
                                {

                                    var subobj_name = pp.x.Name;
                                    if(string.IsNullOrEmpty(subobj_section.Name) == false)
                                    {
                                        subobj_name = subobj_section.Name;
                                    }
                                    var subobj = Activator.CreateInstance(pp.property);
                                    UnPack(ini, subobj, subobj_name);
                                    pp.x.SetValue(obj, subobj, null);
                                }
                                else
                                {
                                    var ddd = FromXml(str, pp.property);
                                    pp.x.SetValue(obj, ddd, null);
                                    //XmlSerializer xml = new XmlSerializer(pp.property);

                                    //using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(str)))
                                    //{
                                    //    var ddd = xml.Deserialize(mm);
                                    //    pp.x.SetValue(obj, ddd, null);
                                    //}
                                }
                            }
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            pp.x.SetValue(obj, Convert.ToInt16(str), null);
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            pp.x.SetValue(obj, Convert.ToInt32(str), null);
                        }
                        break;
                    case TypeCode.Double:
                        {
                            pp.x.SetValue(obj, Convert.ToDouble(str), null);
                        }
                        break;
                    case TypeCode.String:
                        {
                            pp.x.SetValue(obj, str, null);
                        }
                        break;
                    default:
                        {


                        }
                        break;
                }
            }
        }

        static Dictionary<string, Dictionary<string, string>> Parse(string data)
        {
            var dics = new Dictionary<string, Dictionary<string, string>>();
            StringReader strr = new StringReader(data);
            string section = "";
            while (strr.Peek() != -1)
            {
                int idx = -1;

                var str = strr.ReadLine();
                if (string.IsNullOrEmpty(str) == false && str.Length > 2)
                {
                    if (str.First() == '[' && str.Last() == ']')
                    {
                        section = str.Trim('[', ']');
                        dics[section] = new Dictionary<string, string>();
                    }
                    else if (str.First() == ';')
                    {

                    }
                    else if ((idx = str.IndexOf('=')) > 0)
                    {
                        var str_key = str.Substring(0, idx);
                        var str_value = str.Substring(idx + 1, str.Length - (idx + 1));
                        dics[section][str_key] = str_value;
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("");
                    }
                }
            }

            return dics;
        }
    }



    //internal class NativeMethods
    //{
    //    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    //    internal static extern Boolean WritePrivateProfileString(string section, string key, string val, string filePath);
    //    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    //    internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    //}


    //[Obsolete("Old fuction")]
    //public class IniSerializer
    //{
    //    public void Serialize(string section, Dictionary<string, object> datas, string filename)
    //    {
    //        FileInfo fileinfo = new FileInfo(filename);
    //        for (int i = 0; i < datas.Count; i++)
    //        {
    //            object oo = datas.ElementAt(i).Value;
    //            if (oo == null)
    //            {
    //                continue;
    //            }
    //            Type type = oo.GetType();

    //            TypeCode code = Type.GetTypeCode(type);
    //            switch (Type.GetTypeCode(type))
    //            {
    //                case TypeCode.String:
    //                case TypeCode.Int16:
    //                case TypeCode.Int32:
    //                case TypeCode.Int64:
    //                case TypeCode.UInt16:
    //                case TypeCode.UInt32:
    //                case TypeCode.UInt64:
    //                case TypeCode.Double:
    //                case TypeCode.Decimal:
    //                case TypeCode.SByte:
    //                case TypeCode.Byte:
    //                case TypeCode.Single:
    //                case TypeCode.Char:
    //                case TypeCode.Boolean:
    //                case TypeCode.DateTime:
    //                    {
    //                        this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
    //                    }
    //                    break;
    //                case TypeCode.Object:
    //                    {
    //                        if (type.BaseType != null)
    //                        {
    //                            this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
    //                        }
    //                    }
    //                    break;
    //            }
    //        }
    //    }

    //    void WriteINI(string section, string key, object oo, string filename)
    //    {
    //        if (oo != null)
    //        {
    //            Type type = oo.GetType();
    //            TypeCode code = Type.GetTypeCode(type);
    //            switch (Type.GetTypeCode(type))
    //            {
    //                case TypeCode.Double:
    //                    {
    //                        NativeMethods.WritePrivateProfileString(section, key, ((double)oo).ToString("r"), filename);
    //                    }
    //                    break;
    //                case TypeCode.String:
    //                case TypeCode.Int16:
    //                case TypeCode.Int32:
    //                case TypeCode.Int64:
    //                case TypeCode.UInt16:
    //                case TypeCode.UInt32:
    //                case TypeCode.UInt64:
    //                case TypeCode.Decimal:
    //                case TypeCode.SByte:
    //                case TypeCode.Byte:
    //                case TypeCode.Single:
    //                case TypeCode.Char:
    //                case TypeCode.Boolean:
    //                    {
    //                        NativeMethods.WritePrivateProfileString(section, key, oo.ToString(), filename);
    //                    }
    //                    break;
    //                case TypeCode.DateTime:
    //                    {
    //                        DateTime datetime = (DateTime)oo;
    //                        NativeMethods.WritePrivateProfileString(section, key, datetime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo), filename);
    //                    }
    //                    break;
    //                case TypeCode.Object:
    //                    {
    //                        if (oo is TimeSpan)
    //                        {
    //                            NativeMethods.WritePrivateProfileString(section, key, ((TimeSpan)oo).ToString(), filename);
    //                        }
    //                        else
    //                        {

    //                            XmlSerializer xml = new XmlSerializer(type);
    //                            using (MemoryStream mm = new MemoryStream())
    //                            {
    //                                using (StreamWriter sw = new StreamWriter(mm, Encoding.UTF8))
    //                                {
    //                                    using (var xmlWriter = XmlWriter.Create(sw, new XmlWriterSettings { Indent = false, Encoding = Encoding.UTF8 }))
    //                                    {
    //                                        xml.Serialize(xmlWriter, oo);
    //                                    }
    //                                    byte[] bb = mm.ToArray();

    //                                    //because utf-8 begin is 0xef 0xbb 0xbf
    //                                    //remove 3 byte, WritePrivateProfileStringW is no ? at being
    //                                    string str = Encoding.UTF8.GetString(bb, 3, bb.Length - 3);
    //                                    NativeMethods.WritePrivateProfileString(section, key, str, filename);
    //                                }
    //                            }
    //                        }

    //                    }
    //                    break;
    //            }
    //        }
    //        else
    //        {
    //            NativeMethods.WritePrivateProfileString(section, key, "", filename);
    //        }
    //    }

    //    public void Deserialize(string section, Dictionary<string, object> datas, string filename)
    //    {
    //        FileInfo fileinfo = new FileInfo(filename);
    //        for (int i = 0; i < datas.Count; i++)
    //        {
    //            Type type = datas.ElementAt(i).Value?.GetType();
    //            if (type == null)
    //            {
    //                continue;
    //            }
    //            object oo = datas.ElementAt(i).Value;
    //            TypeCode code = Type.GetTypeCode(type);
    //            switch (Type.GetTypeCode(type))
    //            {
    //                case TypeCode.Boolean:
    //                case TypeCode.Byte:
    //                case TypeCode.Char:
    //                case TypeCode.DateTime:
    //                case TypeCode.Decimal:
    //                case TypeCode.Double:
    //                case TypeCode.Int16:
    //                case TypeCode.Int32:
    //                case TypeCode.Int64:
    //                case TypeCode.SByte:
    //                case TypeCode.Single:
    //                case TypeCode.String:
    //                case TypeCode.UInt16:
    //                case TypeCode.UInt32:
    //                case TypeCode.UInt64:
    //                    {
    //                        datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, type, code);
    //                    }
    //                    break;
    //                case TypeCode.Object:
    //                    {
    //                        //if(type.IsArray == true)
    //                        //{
    //                        //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
    //                        //}
    //                        //else if(type.IsClass == true&& type.IsGenericType == true)
    //                        //{
    //                        //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
    //                        //}
    //                        //else if(type.BaseType == null)
    //                        //{

    //                        //}
    //                        //else if(oo is TimeSpan)
    //                        //{
    //                        //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
    //                        //}
    //                        //else
    //                        //{
    //                        //    datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
    //                        //}

    //                        if (type.BaseType != null)
    //                        {
    //                            datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, type, code);
    //                        }
    //                    }
    //                    break;
    //            }
    //        }
    //    }

    //    public object ReadIni(string section, string key, object oo, string filename, Type type, TypeCode typecode)
    //    {
    //        object dst = null;

    //        StringBuilder temp = new StringBuilder(1024);

    //        NativeMethods.GetPrivateProfileString(section, key, "", temp, 255, filename);
    //        if (temp.Length <= 0)
    //        {
    //            return dst;
    //        }
    //        switch (typecode)
    //        {
    //            case TypeCode.Decimal:
    //                {
    //                    Decimal a = 0;
    //                    Decimal.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Boolean:
    //                {
    //                    bool a = false;
    //                    bool.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Byte:
    //                {
    //                    Byte a = 0;
    //                    Byte.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Char:
    //                {
    //                    Char a = Char.MinValue;
    //                    Char.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.SByte:
    //                {
    //                    sbyte a = 0;
    //                    sbyte.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Single:
    //                {
    //                    float a = 0;
    //                    float.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Double:
    //                {
    //                    double a = 0;
    //                    double.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.String:
    //                {
    //                    dst = temp.ToString();
    //                }
    //                break;
    //            case TypeCode.Int16:
    //                {
    //                    Int16 a = 0;
    //                    Int16.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Int32:
    //                {
    //                    Int32 a = 0;
    //                    Int32.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Int64:
    //                {
    //                    Int64 a = 0;
    //                    Int64.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.UInt16:
    //                {
    //                    UInt16 a = 0;
    //                    UInt16.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.UInt32:
    //                {
    //                    UInt32 a = 0;
    //                    UInt32.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.UInt64:
    //                {
    //                    UInt64 a = 0;
    //                    UInt64.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.DateTime:
    //                {
    //                    DateTime a = DateTime.MinValue;
    //                    DateTime.TryParse(temp.ToString(), out a);
    //                    dst = a;
    //                }
    //                break;
    //            case TypeCode.Object:
    //                {
    //                    if (oo is TimeSpan)
    //                    {
    //                        TimeSpan a = new TimeSpan();
    //                        TimeSpan.TryParse(temp.ToString(), out a);
    //                        dst = a;
    //                    }
    //                    else
    //                    {
    //                        string ss = temp.ToString();
    //                        if (string.IsNullOrEmpty(ss) == false && ss.Trim().Length > 0)
    //                        {
    //                            XmlSerializer xml = new XmlSerializer(type);
    //                            Encoding.UTF8.GetBytes(ss);
    //                            using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(ss)))
    //                            {
    //                                dst = xml.Deserialize(mm);
    //                            }
    //                        }
    //                    }
    //                }
    //                break;
    //        }
    //        return dst;
    //    }


    //    public void Serialize(object obj, string filename)
    //    {
    //        FileInfo file = new FileInfo(filename);
    //        Type type = obj.GetType();
    //        TypeCode typecode = Type.GetTypeCode(type);
    //        if (typecode != TypeCode.Object)
    //        {
    //            return;
    //        }

    //        IniSectionAttribute defaultsection = type.GetCustomAttributes(typeof(IniSectionAttribute), false).FirstOrDefault() as IniSectionAttribute;


    //        var pps = type.GetProperties().Where(x => x.CanWrite && x.CanRead);
    //        foreach (PropertyInfo pp in pps)
    //        {
    //            var attrs = pp.GetCustomAttributes(true);
    //            var attribe = pp.GetCustomAttributes(typeof(IniSectionKey), false).FirstOrDefault() as IniSectionKey;
    //            string section = type.Name;
    //            if (defaultsection != null && string.IsNullOrEmpty(defaultsection.DefaultSection) == true && defaultsection.DefaultSection.Trim().Length > 0)
    //            {
    //                section = defaultsection.DefaultSection;
    //            }
    //            string key = pp.Name;
    //            bool ignore = attrs.Any(x => x is IniIgnoreAttribute);
    //            if (attribe != null)
    //            {
    //                if (string.IsNullOrEmpty(attribe.Section) == false && attribe.Section.Trim().Length > 0)
    //                {
    //                    section = attribe.Section;
    //                }
    //                if (string.IsNullOrEmpty(attribe.Key) == false && attribe.Key.Trim().Length > 0)
    //                {
    //                    key = attribe.Key;
    //                }
    //                //ignore = attribe.Ignore;
    //            }
    //            if (ignore == false)
    //            {
    //                this.WriteINI(section, key, pp.GetValue(obj, null), file.FullName);
    //            }
    //        }
    //    }

    //    public void Deserialize(object obj, string filename)
    //    {
    //        FileInfo file = new FileInfo(filename);
    //        Type type = obj.GetType();
    //        TypeCode typecode = Type.GetTypeCode(type);
    //        if (typecode != TypeCode.Object)
    //        {
    //            return;
    //        }
    //        var defaultsection = type.GetCustomAttributes(typeof(IniSectionAttribute), false).FirstOrDefault() as IniSectionAttribute;

    //        var pps = type.GetProperties().Where(x => x.CanWrite && x.CanRead);
    //        foreach (PropertyInfo pp in pps)
    //        {
    //            var attrs = pp.GetCustomAttributes(true);
    //            var attribe = pp.GetCustomAttributes(typeof(IniSectionKey), false).FirstOrDefault() as IniSectionKey;
    //            string section = type.Name;
    //            if (defaultsection != null && string.IsNullOrEmpty(defaultsection.DefaultSection) == true && defaultsection.DefaultSection.Trim().Length > 0)
    //            {
    //                section = defaultsection.DefaultSection;
    //            }
    //            string key = pp.Name;
    //            bool ignore = attrs.Any(x => x is IniIgnoreAttribute);
    //            if (attribe != null)
    //            {
    //                if (string.IsNullOrEmpty(attribe.Section) == false && attribe.Section.Trim().Length > 0)
    //                {
    //                    section = attribe.Section;
    //                }
    //                if (string.IsNullOrEmpty(attribe.Key) == false && attribe.Key.Trim().Length > 0)
    //                {
    //                    key = attribe.Key;
    //                }
    //                //ignore = attribe.Ignore;
    //            }
    //            if (ignore == false)
    //            {
    //                object src = pp.GetValue(obj, null);
    //                object dst = this.ReadIni(section, key, src, file.FullName, pp.PropertyType, Type.GetTypeCode(pp.PropertyType));
    //                pp.SetValue(obj, dst, null);
    //            }
    //        }
    //    }
    //}

}
