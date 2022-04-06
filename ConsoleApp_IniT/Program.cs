using QSoft.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace ConsoleApp_IniT
{
    public class IniBuilder<T>
    {
        Dictionary<PropertyInfo, string> m_Items = new Dictionary<PropertyInfo, string>();


        public IniPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<T, TProperty>> func)
        {
            return new IniPropertyBuilder<TProperty>();
        }
    }

    public class IniPropertyBuilder<T>
    {
        string m_Annotation = "";
        string m_PropertyName = "";
        bool m_Ignore = false;
        public IniPropertyBuilder<T> HasPropertyName(string data)
        {
            return this;
        }
        public IniPropertyBuilder<T> HasAnnotation(string data)
        {
            return this;
        }
        public IniPropertyBuilder<T> HasIgnore()
        {
            return this;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            IniBuilder<Setting> model = new IniBuilder<Setting>();
            var pd = model.Property(x => x.Port);


Setting setting = new Setting()
{
    IP = "127.0.0.1",
    Port = 88,
    //TestItems1 = new List<TestItem>()
    //{
    //    new TestItem(){Exe = "A.exe", Command="qq"},
    //    new TestItem(){Exe="B.bat", Command=""},
    //    new TestItem(){Exe="taskmgr.exe"}
    //}
};

            //setting.Ftp_1 = new RemoteSetting() { IP = "127.0.0.100", Port = 50, Account = "Allen", Password = "123" };
            //setting.FTP_Log = new RemoteSetting() { IP = "192.168.10.100", Port = 96, Account = "Julia", Password = "456" };
            //setting.FTP_Backup = new RemoteSetting() { IP = "168.55.55.55", Port = 12, Account = "David", Password = "789" };

            //setting.TestItems1 = new List<TestItem>()
            //{
            //    new TestItem(){Exe = "A.exe", Command="qq"},
            //    new TestItem(){Exe="B.bat", Command=""},
            //    new TestItem(){Exe="taskmgr.exe"}
            //};

            //setting.TestItems2 = new List<TestItem>()
            //{
            //    new TestItem(){Exe = "A2.exe", Command="qq2"},
            //    new TestItem(){Exe="B2.bat", Command=""},
            //    new TestItem(){Exe="taskmgr2.exe"}
            //};



            string ini_str = IniConvert.SerializeObject(setting);
            File.WriteAllText("setting.ini", ini_str);
            var inides = IniConvert.DeserializeObject<Setting>(ini_str);


        }
    }

[IniAnnotation(Annotation = "2022/02/02 modify")]
public class Setting
{
    [IniAnnotation(Annotation = "127.0.0.1 is default")]
    [IniAnnotation(Annotation = "Please check it")]
    public string IP { set; get; }
    public int Port { set; get; }
    //[IniArray(Name ="Test")]
    //public List<TestItem> TestItems1 { set; get; }
}

    //[QSoft.Ini.IniSection()]
    //public class Setting
    //{
    //    public string Title { set; get; } = "Plan1";
    //    public DateTime ModifyTime { set; get; } = DateTime.Now;
    //    public TimeSpan TimeOut { set; get; } = TimeSpan.FromSeconds(30);

    //    public RemoteSetting Ftp_1 { set; get; }
    //    [IniSectionKey(Key ="FTP_2")]
    //    public RemoteSetting FTP_Log { set; get; }
    //    [IniSection()]
    //    public RemoteSetting FTP_Backup { set; get; }

    //    [IniArray]
    //    public List<TestItem> TestItems1 { set; get; }

    //    [IniArray(Name="Demo", BaseIndex =10)]
    //    public List<TestItem> TestItems2 { set; get; }
    //}

    public class TestItem
    {
        public string Exe { set; get; }
        public string Command { set; get; }
    }

    public class RemoteSetting
    {
        public string IP { set; get; }
        public int Port { set; get; }
        public string Account { set; get; }
        public string Password { set; get; }
    }



    
}
