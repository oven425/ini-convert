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
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("string", "string");
            dic.Add("int", 10);
            dic.Add("CTest", new CTest());
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
        public string A { set; get; } = "A";
        public int B { set; get; } = 100;
    }
}
