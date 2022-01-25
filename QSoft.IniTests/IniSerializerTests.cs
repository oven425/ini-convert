using Microsoft.VisualStudio.TestTools.UnitTesting;
using QSoft.Ini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.Ini.Tests
{
    [TestClass()]
    public class IniSerializerTests
    {
        [TestMethod()]
        public void Max()
        {
            DataMax obj = new DataMax();
            var section1 = obj.GetType().GetCustomAttributes(true).FirstOrDefault(x => x.GetType() == typeof(IniSection)) as IniSection;
            Dictionary<string, Dictionary<string, string>> sections = new Dictionary<string, Dictionary<string, string>>();
            List<string> sectionlist = new List<string>();
            var methods = typeof(IniConvert).GetMethod("Pack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            methods.Invoke(null, new object[] { obj, section1 == null ? obj.GetType().Name : section1.DefaultSection, sections, sectionlist });
            string inifile = @"C:\Users\ben_hsu\source\repos\QSoft.Ini\QSoft.IniTests\bin\Debug\native.ini";
            foreach(var oo in sectionlist)
            {
                foreach(var item in sections[oo])
                {
                    NativeMethods.WritePrivateProfileString(oo, item.Key, item.Value, inifile);
                }
            }

        }

        [TestMethod()]
        public void Min()
        {

        }
    }

    public class NativeMethods
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern Boolean WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
    }

    public class DataMax
    {
        public char Char { set; get; } = char.MaxValue;
        public byte Byte { set; get; } = byte.MaxValue;
        public short Short { set; get; } = short.MaxValue;
        public ushort UShort { set; get; } = ushort.MaxValue;
        public int Int { set; get; } = int.MaxValue;
        public uint UInt { set; get; } = uint.MaxValue;
        public long Long { set; get; } = long.MaxValue;
        public ulong ULnt { set; get; } = ulong.MaxValue;
        public float Float { set; get; } = float.MaxValue;
        public double Double { set; get; } = double.MaxValue;
        public decimal Decimal { set; get; } = decimal.MaxValue;
        public DateTime DateTime { set; get; } = DateTime.MaxValue;
        public TimeSpan TimeSpan { set; get; } = TimeSpan.MaxValue;
    }

    public class DataMin
    {
        public char Char { set; get; } = char.MinValue;
        public byte Byte { set; get; } = byte.MinValue;
        public short Short { set; get; } = short.MinValue;
        public ushort UShort { set; get; } = ushort.MinValue;
        public int Int { set; get; } = int.MinValue;
        public uint UInt { set; get; } = uint.MinValue;
        public long Long { set; get; } = long.MinValue;
        public ulong ULnt { set; get; } = ulong.MinValue;
        public float Float { set; get; } = float.MinValue;
        public double Double { set; get; } = double.MinValue;
        public decimal Decimal { set; get; } = decimal.MinValue;
        public DateTime DateTime { set; get; } = DateTime.MinValue;
        public TimeSpan TimeSpan { set; get; } = TimeSpan.MinValue;
    }
}