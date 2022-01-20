using Microsoft.VisualStudio.TestTools.UnitTesting;
using QSoft.Ini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSoft.Ini.Tests
{
    [TestClass()]
    public class IniSerializerTests
    {
        [TestMethod()]
        public void Ini()
        {
            Assert.Fail();
        }

        public class Setting
        {

        }

        public class User
        {
            public string Name { set; get; }
            public int Age { set; get; }
            public List<string> Phones { set; get; }
        }
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