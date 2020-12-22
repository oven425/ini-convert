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
            //dic.Add("Array", new int[] { 1, 2, 3 });
            //dic.Add("List", new List<int>() { 1,2,3});
            //dic.Add("decimal", new decimal(100));
            //dic.Add("Now", DateTime.Now);
            //dic.Add("char", char.MaxValue);
            //dic.Add("byte", byte.MaxValue);
            //dic.Add("short", short.MaxValue);
            //dic.Add("string", "答案");
            //dic.Add("int", 10);
            //dic.Add("CTest", new CTest() { A = "答案A", B = 101, Test1 = new CTest_1() { A1 = "答案B", B1 = 202 } });
            //ini.Serialize("dic", dic, "test.ini");


            Dictionary<string, object> dic1 = new Dictionary<string, object>();
            dic1.Add("Array", new int[1]);
            dic1.Add("List", new List<int>());
            dic1.Add("Now", new DateTime());
            dic1.Add("string", "");
            dic1.Add("int", 0);
            dic1.Add("CTest", new CTest() { });
            ini.Deserialize("dic", dic1, "test.ini");
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
