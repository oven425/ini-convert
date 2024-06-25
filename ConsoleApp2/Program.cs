using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            //HelloFrom("Generated Code");
            People pp = new People();
            var aa = pp.GetInitString();
            //(?<month>\d{1,2})
            //var regex_section = new Regex(@"\[\w+\]");

            var cotent = @"[Test]
IP=127.0.0.1
Port=88
Account=KK
Password=poiuu
";

            var regex_section = new Regex(@"^\[(?<section>\w+)\]$");
            var bb = regex_section.Match("[aaaa]");
            if(bb.Success)
            {
                var gg = bb.Groups["section"].Value;
            }
            var inir = new IniReader(new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(cotent))));

            while(inir.Read())
            {
                switch(inir.TokenType)
                {
                    case IniTokenType.Section:
                        {

                        }
                        break;

                }
            }
            Console.ReadLine();

           
        }

        static partial void HelloFrom(string name);
    }

    public enum IniTokenType
    {
        None,
        Section,
        Commet,
        PropertyName,
        Value
    }

    public class IniReader
    {
        StreamReader m_Reader;
        public IniReader(StreamReader sr)
        {
            this.m_Reader = sr;
        }
        Regex regex_section = new(@"^\[(?<section>\w+)\]$");
        public IniTokenType TokenType {  get; private set; }
        public string GetString()
        {
            return "";
        }

        public bool Read()
        {
            var line = m_Reader.ReadLine();
            if(this.TokenType == IniTokenType.None)
            {
                var match = regex_section.Match(line);
                if(match.Success)
                {
                    TokenType = IniTokenType.Section;
                    m_Section = match.Groups["section"].Value;
                }
            }
            
            return true;
        }

        string m_Section;

        public string ReadString()
        {
            if(this.TokenType == IniTokenType.Section)
            {
                return m_Section;
            }
            return "";
        }


        public string ReadSectionName()
        {
            return "";
        }

    }

    public partial class AugmentClass
    {
        public void AugmentMethod()
        {
            // 调用代码生成器中的方法
            //this.GeneratedMethod();
        }
    }

    [Description("AA")]
    [IniSection]
    public partial class People 
    {
        public string Name { set; get; }
        public int Age { set; get; }
        public string Account { set; get; }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class IniSectionAttribute:Attribute
    {
        //public string Name {  get; set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class IniIgnoreAttribute : Attribute
    {

    }

}
