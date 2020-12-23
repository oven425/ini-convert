using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace IniConvert
{
    public class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern Boolean WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }
    public class IniSerializer
    {
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
                            if(type.IsGenericType == true)
                            {
                                this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
                            }
                            else if(type.IsArray == true)
                            {
                                this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
                            }
                            else if(type.BaseType == null)
                            {

                            }
                            else if (oo is TimeSpan)
                            {
                                this.WriteINI(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName);
                            }
                            else
                            {
                                PropertyInfo[] pps = type.GetProperties();
                                foreach (var pp in pps)
                                {
                                    this.WriteINI(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value), fileinfo.FullName);
                                }
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
                                NativeMethods.WritePrivateProfileString(section, key, ((TimeSpan)oo).ToString("c"), filename);
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
                            datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            if(type.IsArray == true)
                            {
                                datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            }
                            else if(type.IsClass == true&& type.IsGenericType == true)
                            {
                                datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            }
                            else if(type.BaseType == null)
                            {

                            }
                            else if(oo is TimeSpan)
                            {
                                datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Key, datas.ElementAt(i).Value, fileinfo.FullName, code);
                            }
                            else
                            {
                                PropertyInfo[] pps = type.GetProperties();
                                foreach (var pp in pps)
                                {
                                    string sss = pp.Name;
                                    TypeCode code1 = Type.GetTypeCode(pp.PropertyType);

                                    if (code1 == TypeCode.Object)
                                    {
                                        object oo1 = this.ReadIni(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value), fileinfo.FullName, code1);
                                        pp.SetValue(datas.ElementAt(i).Value, oo1);
                                    }
                                    else
                                    {
                                        object oo1 = this.ReadIni(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value, null), fileinfo.FullName, code1);
                                        pp.SetValue(datas.ElementAt(i).Value, oo1);
                                    }

                                }
                            }
                        }
                        break;
                }
            }
        }

        public object ReadIni(string section, string key, object oo, string filename, TypeCode typecode)
        {
            
            object dst = null;
            if (oo == null)
            {
                return dst;
            }
            StringBuilder temp = new StringBuilder(255);
            NativeMethods.GetPrivateProfileString(section, key, "", temp, 255, filename);
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
                            XmlSerializer xml = new XmlSerializer(oo.GetType());
                            string ss = temp.ToString();
                            Encoding.UTF8.GetBytes(ss);
                            using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(ss)))
                            {
                                dst = xml.Deserialize(mm);
                            }
                        }
                        
                    }
                    break;
            }
            return dst;
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
            IniSection defaultsection = type.GetCustomAttributes<IniSection>().FirstOrDefault();
            if(defaultsection == null || string.IsNullOrEmpty(defaultsection.DefaultSection)==true)
            {
                return;
            }
            PropertyInfo[] pps = type.GetProperties();
            foreach(PropertyInfo pp in pps)
            {
                var attribe = pp.GetCustomAttributes<IniSectionKey>().FirstOrDefault();
                string section = defaultsection.DefaultSection;
                string key = pp.Name;
                bool ignore = false;
                if (attribe != null)
                {
                    if(string.IsNullOrWhiteSpace(attribe.Section) == false)
                    {
                        section = attribe.Section;
                    }
                    if (string.IsNullOrWhiteSpace(attribe.Key) == false)
                    {
                        key = attribe.Key;
                    }
                    ignore = attribe.Ignore;
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
            IniSection defaultsection = type.GetCustomAttributes<IniSection>().FirstOrDefault();
            if (defaultsection == null || string.IsNullOrEmpty(defaultsection.DefaultSection) == true)
            {
                return;
            }
            PropertyInfo[] pps = type.GetProperties();
            foreach (PropertyInfo pp in pps)
            {
                var attribe = pp.GetCustomAttributes<IniSectionKey>().FirstOrDefault();
                string section = type.Name;
                string key = pp.Name;
                bool ignore = false;
                if (attribe != null)
                {
                    if (string.IsNullOrWhiteSpace(attribe.Section) == false)
                    {
                        section = attribe.Section;
                    }
                    if (string.IsNullOrWhiteSpace(attribe.Key) == false)
                    {
                        key = attribe.Key;
                    }
                    ignore = attribe.Ignore;
                }
                if (ignore == false)
                {
                    object dst = this.ReadIni(section, key, pp.GetValue(obj, null), file.FullName, Type.GetTypeCode(pp.PropertyType));
                    pp.SetValue(obj, dst);
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
        public bool Ignore { set; get; }
    }
}
