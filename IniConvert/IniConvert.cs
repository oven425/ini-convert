using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
                        {
                            WritePrivateProfileString(datas.ElementAt(i).Key, type.Name, datas.ElementAt(i).Value.ToString(), fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        {
                            WritePrivateProfileString(datas.ElementAt(i).Key, type.Name, datas.ElementAt(i).Value.ToString(), fileinfo.FullName);
                        }
                        break;
                    case TypeCode.Object:
                        {
                            PropertyInfo[] pps = type.GetProperties();
                            foreach (var pp in pps)
                            {
                                object ooj = pp.GetValue(datas.ElementAt(i).Value);
                                WritePrivateProfileString(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value, null).ToString(), fileinfo.FullName);
                            }
                        }
                        break;
                }
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
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            string str1 = datas.ElementAt(i).Value as string;
                            datas[datas.ElementAt(i).Key] = str;
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            Int16 a = 0;
                            if (Int16.TryParse(str, out a) == true)
                            {
                                datas[datas.ElementAt(i).Key] = a;
                            }
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            Int32 a = 0;
                            if(Int32.TryParse(str, out a) == true)
                            {
                                datas[datas.ElementAt(i).Key] = a;
                            }
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            Int64 a = 0;
                            if (Int64.TryParse(str, out a) == true)
                            {
                                datas[datas.ElementAt(i).Key] = a;
                            }
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            UInt16 a = 0;
                            if (UInt16.TryParse(str, out a) == true)
                            {
                                datas[datas.ElementAt(i).Key] = a;
                            }
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            UInt32 a = 0;
                            if (UInt32.TryParse(str, out a) == true)
                            {
                                datas[datas.ElementAt(i).Key] = a;
                            }
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            string str = this.ReadIni(datas.ElementAt(i).Key, type.Name, fileinfo.FullName);
                            UInt64 a = 0;
                            if (UInt64.TryParse(str, out a) == true)
                            {
                                datas[datas.ElementAt(i).Key] = a;
                            }
                        }
                        break;
                    case TypeCode.Object:
                        {
                            PropertyInfo[] pps = type.GetProperties();
                            foreach (var pp in pps)
                            {
                                object ooj = pp.GetValue(datas.ElementAt(i).Value);
                                WritePrivateProfileString(datas.ElementAt(i).Key, pp.Name, pp.GetValue(datas.ElementAt(i).Value, null).ToString(), fileinfo.FullName);
                            }
                        }
                        break;
                }
            }
        }

        void Read()
        {

        }

        public string ReadIni(string section, string key, string filepath)
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, filepath);
            return temp.ToString();
        }
    }
}
