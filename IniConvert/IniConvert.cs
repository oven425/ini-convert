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
                            this.WriteINI(section,datas.ElementAt(i).Value, datas.ElementAt(i).Key, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            if(type.IsGenericType == true)
                            {
                                this.WriteINI(section, datas.ElementAt(i).Value, datas.ElementAt(i).Key, fileinfo.FullName);
                            }
                            else if(type.IsArray == true)
                            {
                                this.WriteINI(section, datas.ElementAt(i).Value, datas.ElementAt(i).Key, fileinfo.FullName);
                            }
                            else
                            {
                                PropertyInfo[] pps = type.GetProperties();
                                foreach (var pp in pps)
                                {
                                    object ooj = pp.GetValue(datas.ElementAt(i).Value);
                                    this.WriteINI(datas.ElementAt(i).Key, pp.GetValue(datas.ElementAt(i).Value), pp.Name, fileinfo.FullName);
                                }
                            }
                            
                        }
                        break;
                }
            }
        }

        void WriteINI(string key, object oo, string typename, string  filename)
        {
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
                    {
                        NativeMethods.WritePrivateProfileString(key, typename, oo.ToString(), filename);
                    }
                    break;
                case TypeCode.DateTime:
                    {
                        DateTime datetime = (DateTime)oo;
                        NativeMethods.WritePrivateProfileString(key, typename, datetime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo), filename);
                    }
                    break;
                case TypeCode.Object:
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
                                string str = Encoding.UTF8.GetString(bb, 3, bb.Length-3);

                                System.Diagnostics.Trace.WriteLine(str);
                                NativeMethods.WritePrivateProfileString(key, typename, str, filename);
                            }
                        }
                    }
                    break;
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
                            datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Value, datas.ElementAt(i).Key, fileinfo.FullName, code);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            if(type.IsArray == true)
                            {
                                datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Value, datas.ElementAt(i).Key, fileinfo.FullName, code);
                            }
                            else if(type.IsClass == true&& type.IsGenericType == true)
                            {
                                datas[datas.ElementAt(i).Key] = this.ReadIni(section, datas.ElementAt(i).Value, datas.ElementAt(i).Key, fileinfo.FullName, code);
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
                                        object oo1 = this.ReadIni(datas.ElementAt(i).Key, pp.GetValue(datas.ElementAt(i).Value), pp.Name, fileinfo.FullName, code1);
                                        pp.SetValue(datas.ElementAt(i).Value, oo1);
                                    }
                                    else
                                    {
                                        object oo1 = this.ReadIni(datas.ElementAt(i).Key, pp.GetValue(datas.ElementAt(i).Value, null), pp.Name, fileinfo.FullName, code1);
                                        pp.SetValue(datas.ElementAt(i).Value, oo1);
                                    }

                                }
                            }
                        }
                        break;
                }
            }
        }

        public object ReadIni(string key, object oo, string typename, string filename, TypeCode typecode)
        {
            object dst = null;
            StringBuilder temp = new StringBuilder(255);
            NativeMethods.GetPrivateProfileString(key, typename, "", temp, 255, filename);
            switch (typecode)
            {
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
                        XmlSerializer xml = new XmlSerializer(oo.GetType());
                        string ss = temp.ToString();
                        Encoding.UTF8.GetBytes(ss);
                        using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(ss)))
                        {
                            dst = xml.Deserialize(mm);
                        }
                    }
                    break;
            }
            return dst;
        }
    }

    public class CIniIgnore
    {

    }
}
