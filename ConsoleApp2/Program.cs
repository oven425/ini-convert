using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using QSoft.Ini;

namespace ConsoleApp2
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            People pp = new People();
            //HelloFrom("Generated Code");
            var aa = pp.GetInitString();
            //(?<month>\d{1,2})

            //            var cotent = @"[Test]
            //IP=127.0.0.1
            //Port=88
            //Account=KK
            //Password=poiuu
            //";
            //            var regex_keyvalue = new Regex(@"^(?<key>\w+)=(?<value>.*)$");
            //            var bbb = regex_keyvalue.Match("IP=127.0.0.1");
            //            if (bbb.Success)
            //            {
            //                var key = bbb.Groups["key"].Value;
            //                var value = bbb.Groups["value"].Value;
            //            }

            //            var regex_section = new Regex(@"^\[(?<section>.*)\]$");
            //            var bb = regex_section.Match("[1.2.3]");
            //            if(bb.Success)
            //            {
            //                var gg = bb.Groups["section"].Value;
            //            }
            //            var inir = new IniReader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(cotent))));

            //            while(inir.Read())
            //            {
            //                switch(inir.TokenType)
            //                {
            //                    case IniTokenType.Section:
            //                        {
            //                            System.Diagnostics.Trace.WriteLine($"[{inir.GetString()}]");
            //                        }
            //                        break;
            //                    case IniTokenType.PropertyName:
            //                        {
            //                            System.Diagnostics.Trace.Write($"{inir.GetString()}=");
            //                        }
            //                        break;
            //                    case IniTokenType.Value:
            //                        {
            //                            System.Diagnostics.Trace.Write($"{inir.GetString()}");
            //                            System.Diagnostics.Trace.WriteLine("");
            //                        }
            //                        break;

            //                }
            //            }
            Console.ReadLine();

           
        }
        
        static partial void HelloFrom(string name);
    }

    

    

    public partial class AugmentClass
    {
        public void AugmentMethod()
        {
            // 调用代码生成器中的方法
            //this.GeneratedMethod();
        }
    }

    [IniSection]
    public partial class People 
    {
        public string Name { set; get; }
        public int Age { set; get; }
        public string Account { set; get; }
    }

    //[System.AttributeUsage(System.AttributeTargets.Class)]
    //public class IniSectionAttribute:Attribute
    //{
    //    //public string Name {  get; set; }
    //}

    //[System.AttributeUsage(System.AttributeTargets.Property)]
    //public class IniIgnoreAttribute : Attribute
    //{

    //}

}
