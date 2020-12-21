using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_IniT
{
    class Program
    {
        static void Main(string[] args)
        {
            IniConvert.IniSerializer ini = new IniConvert.IniSerializer();

            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.Add("char", char.MaxValue);
            //dic.Add("byte", byte.MaxValue);
            //dic.Add("short", short.MaxValue);
            //dic.Add("string", "string");
            //dic.Add("int", 10);
            //dic.Add("CTest", new CTest() { A = "AA", B = 101, Test1 = new CTest_1() { A1 = "AA11", B1 = 202 } });
            //ini.Serialize(dic, "test.ini");

            Dictionary<string, object> dic1 = new Dictionary<string, object>();
            dic1.Add("string", "");
            dic1.Add("int", 0);
            dic1.Add("CTest", new CTest());
            ini.Deserialize(dic1, "test.ini");
        }

    }

    public class CTest
    {
        public string A { set; get; }
        public int B { set; get; }
        public CTest_1 Test1 { set; get; } = new CTest_1();
    }

    public class CTest_1
    {
        public string A1 { set; get; }
        public int B1 { set; get; }
    }
}
