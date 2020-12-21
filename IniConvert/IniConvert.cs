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
    public class IniSerializer
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public void Serialize(Dictionary<string, object> datas, string filename)
        {
            FileInfo fileinfo = new FileInfo(filename);
            
            for (int i=0; i< datas.Count; i++)
            {
                Type type = datas.ElementAt(i).Value.GetType();
                object oo = datas.ElementAt(i).Value;
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
                            this.WriteINI(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            PropertyInfo[] pps = type.GetProperties();
                            foreach (var pp in pps)
                            {
                                object ooj = pp.GetValue(datas.ElementAt(i).Value);
                                this.WriteINI(datas.ElementAt(i).Key, pp.GetValue(datas.ElementAt(i).Value), pp.Name, fileinfo.FullName);
                                //WritePrivateProfileString(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value, null).ToString(), fileinfo.FullName);
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
                        WritePrivateProfileString(key, typename, oo.ToString(), filename);
                    }
                    break;
                case TypeCode.DateTime:
                    {
                        DateTime datetime = (DateTime)oo;
                        WritePrivateProfileString(key, typename, datetime.ToString("yyyy-MM-dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo), filename);
                    }
                    break;
                case TypeCode.Object:
                    {
                        XmlSerializer xml = new XmlSerializer(type);
                        using (MemoryStream mm = new MemoryStream())
                        {
                            using (var xmlWriter = XmlWriter.Create(mm, new XmlWriterSettings { Indent = false }))
                            {
                                xml.Serialize(xmlWriter, oo);
                            }
                            byte[] bb = mm.ToArray();
                            string str = Encoding.UTF8.GetString(bb);
                            WritePrivateProfileString(key, type.Name, str, filename);
                        }
                    }
                    break;
            }
        }

        public void Deserialize(Dictionary<string, object> datas, string filename)
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
                    case TypeCode.String:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //string str1 = datas.ElementAt(i).Value as string;
                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //Int16 a = 0;
                            //if (Int16.TryParse(str, out a) == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = a;
                            //}

                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //Int32 a = 0;
                            //if(Int32.TryParse(str, out a) == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = a;
                            //}
                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //Int64 a = 0;
                            //if (Int64.TryParse(str, out a) == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = a;
                            //}
                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //UInt16 a = 0;
                            //if (UInt16.TryParse(str, out a) == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = a;
                            //}
                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //UInt32 a = 0;
                            //if (UInt32.TryParse(str, out a) == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = a;
                            //}
                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            //string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            //UInt64 a = 0;
                            //if (UInt64.TryParse(str, out a) == true)
                            //{
                            //    datas[datas.ElementAt(i).Key] = a;
                            //}
                            datas[datas.ElementAt(i).Key] = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            PropertyInfo[] pps = type.GetProperties();
                            foreach (var pp in pps)
                            {
                                string sss = pp.Name;
                                TypeCode code1 = Type.GetTypeCode(pp.PropertyType);
                                if(code1 == TypeCode.Object)
                                {
                                    object oo1 = this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                                }
                                else
                                {
                                    object oo1= this.ReadIni(datas.ElementAt(i).Key, datas.ElementAt(i).Value, type.Name, fileinfo.FullName);
                                    pp.SetValue(datas.ElementAt(i).Value, oo1);
                                    //object ooj = pp.GetValue(datas.ElementAt(i).Value);
                                    //WritePrivateProfileString(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value, null).ToString(), fileinfo.FullName);
                                }
                                
                            }
                        }
                        break;
                }
            }
        }

        public object ReadIni(string key, object oo, string typename, string filename)
        {
            object dst = null;
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(key, typename, "", temp, 255, filename);
            TypeCode code = Type.GetTypeCode(oo.GetType());
            switch (code)
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
                case TypeCode.Object:
                    {
                        //XmlSerializer xml = new XmlSerializer(oo.GetType());
                        //string ss = temp.ToString();
                        //Encoding.UTF8.GetBytes(ss);
                        //using (MemoryStream mm = new MemoryStream(Encoding.UTF8.GetBytes(ss)))
                        //{
                        //    dst = xml.Deserialize(mm);
                        //}
                    }
                    break;
            }
            return dst;
        }

        public T ReadIni<T>(string key, object oo, string typename, string filename)
        {
            T hr = default(T);
            TypeCode code = Type.GetTypeCode(typeof(T));
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(key, typename, "", temp, 255, filename);
            switch (code)
            {
                case TypeCode.String:
                    {
                        hr = (T)Convert.ChangeType(temp.ToString(), code);
                    }
                    break;
                case TypeCode.Int16:
                    {
                        Int16 a = 0;
                        if (Int16.TryParse(temp.ToString(), out a) == true)
                        {
                            hr = (T)Convert.ChangeType(a, code);
                        }
                    }
                    break;
                case TypeCode.Int32:
                    {
                        Int32 a = 0;
                        if (Int32.TryParse(temp.ToString(), out a) == true)
                        {
                            hr = (T)Convert.ChangeType(a, code);
                        }
                    }
                    break;
                case TypeCode.Int64:
                    {
                        Int64 a = 0;
                        if (Int64.TryParse(temp.ToString(), out a) == true)
                        {
                            hr = (T)Convert.ChangeType(a, code);
                        }
                    }
                    break;
                case TypeCode.UInt16:
                    {
                        UInt16 a = 0;
                        if (UInt16.TryParse(temp.ToString(), out a) == true)
                        {
                            hr = (T)Convert.ChangeType(a, code);
                        }
                    }
                    break;
                case TypeCode.UInt32:
                    {
                        UInt32 a = 0;
                        if (UInt32.TryParse(temp.ToString(), out a) == true)
                        {
                            hr = (T)Convert.ChangeType(a, code);
                        }
                    }
                    break;
                case TypeCode.UInt64:
                    {
                        UInt64 a = 0;
                        if (UInt64.TryParse(temp.ToString(), out a) == true)
                        {
                            hr = (T)Convert.ChangeType(a, code);
                        }
                    }
                    break;
            }
            return hr;
        }
        public string ReadIni(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, filepath);
            return temp.ToString();
        }
    }
}
