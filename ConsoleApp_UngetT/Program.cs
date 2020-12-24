using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_UngetT
{
    class Program
    {
        static void Main(string[] args)
        {
            QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();
            ini.Serialize("ss", new Dictionary<string, object>(), "test.ini");
        }
    }
}
