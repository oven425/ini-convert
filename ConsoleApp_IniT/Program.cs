using QSoft.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace ConsoleApp_IniT
{
    public class IniModelBuilder<T> where T:class
    {
        string m_Annotation = "";
        Dictionary<string, PropertyInfo> m_Items;
        public Dictionary<string, IniPropertyBuilder> PropertyBuilds { private set; get; } = new Dictionary<string, IniPropertyBuilder>();
        public IniModelBuilder()
        {
            this.m_Items = typeof(T).GetProperties().ToDictionary(x => x.Name);
        }

        public IniModelBuilder<T> HasAnnotation(string data)
        {
            this.m_Annotation = data;
            return this;
        }

        public IniPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<T, TProperty>> func)
        {
            var aa = func.Body as MemberExpression;
            if (aa.Expression.Type == typeof(T))
            {
                if (m_Items.ContainsKey(aa.Member.Name) == true)
                {
                    var pb = new IniPropertyBuilder<TProperty>(aa.Member.Name);
                    this.PropertyBuilds[aa.Member.Name] = pb;
                    return pb;
                }
            }
            return null;
        }

        public IniPropertyBuilder Property(string name)
        {
            if (this.m_Items.ContainsKey(name) == true)
            {
                var typedef = typeof(IniPropertyBuilder<>).MakeGenericType(this.m_Items[name].PropertyType);
                var pb =Activator.CreateInstance(typedef, name) as IniPropertyBuilder;
                this.PropertyBuilds[name] = pb;
                return pb;
            }
            return null;
        }

        public IniPropertyBuilder<TProperty> Property<TProperty>(string name)
        {
            if (this.m_Items.ContainsKey(name) == true)
            {
                var pb = new IniPropertyBuilder<TProperty>(name);
                this.PropertyBuilds[name] = pb;
                return pb;
            }
            return null;
        }

    }

    //https://github.com/dotnet/efcore/blob/main/src/EFCore/Metadata/Builders/PropertyBuilder.cs
    public class IniPropertyBuilder
    {
        public string Annotation { protected set; get; }
        public string PropertyName { protected set; get; }
        public bool Ignore { protected set; get; }
        public int BaseIndex { protected set; get; }
        public IniPropertyBuilder()
        {

        }
        public IniPropertyBuilder(string name)
        {
            this.PropertyName = name;
        }
        public IniPropertyBuilder HasPropertyName(string data)
        {
            this.PropertyName = data;
            return this;
        }
        public IniPropertyBuilder HasAnnotation(string data)
        {
            this.Annotation = data;
            return this;
        }
        public IniPropertyBuilder HasIgnore()
        {
            this.Ignore = true;
            return this;
        }

        public IniPropertyBuilder HasArray(string name, int baseindex=0)
        {
            this.PropertyName = name;
            this.BaseIndex = baseindex;
            return this;
        }

        public IniPropertyBuilder HasConvert(Func<object, object> write, Func<object, object> read)
        {
            this.m_ConvertTo = write;
            this.m_ConvertBack = read;
            return this;
        }
        protected Func<object, object> m_ConvertTo;
        protected Func<object, object> m_ConvertBack;
    }

    public class IniPropertyBuilder<T> : IniPropertyBuilder
    {
        public IniPropertyBuilder(string name)
            : base(name)
        {

        }
        LambdaExpression m_ConvertBack;
        Delegate m_Convert;
        Type m_In;
        Type m_Out;
        public IniPropertyBuilder<T> HasConvert<T1>(Expression<Func<T, T1>> convert, Expression<Func<T1, T>> convertback)
        {
            this.m_Convert = convert.Compile();
            var sss = m_Convert.DynamicInvoke(11);
            this.m_In = typeof(T);
            this.m_Out = typeof(T1);

            var functype = typeof(Func<,>).MakeGenericType(convert.Type.GetGenericArguments());
            
            var method = convert.Compile();
            //m_ConvertTo = method;
            return this;
        }
    }

    class Program
    {
        static void Acc(string name, Type type)
        {
            name = "123";
            type = typeof(int);
        }
        static void Main(string[] args)
        {
            IniModelBuilder<Setting> model = new IniModelBuilder<Setting>();
            var modela = typeof(Setting).GetCustomAttributes(typeof(IniAnnotation), true).FirstOrDefault();
            model.HasAnnotation((modela as IniAnnotation)?.Annotation);
            model.Property(x => x.Port)
                .HasConvert(x => $"{x:X}", x => int.Parse(x))
                .HasAnnotation("Port default is 3333");
            foreach (var pp in typeof(Setting).GetProperties())
            {
                model.Property(x => x.Port).HasConvert(x => x.ToString(), x => int.Parse(x));
                //model.TestAction<Type>(Acc);
                //model.Property((string name, int a) => { name = pp.Name; a = 1; });
                //var pb1 = model.Property(x=> { pp.Name; pp.PropertyType; });
                var pb = model.Property(pp.Name);
                pb.HasConvert(x => x.ToString(), x => int.Parse(x.ToString()));
                var attrs = pp.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    if (attr is IniIgnore)
                    {
                        pb.HasIgnore();
                    }
                    else if (attr is IniAnnotation)
                    {
                        pb.HasAnnotation((attr as IniAnnotation)?.Annotation);
                    }
                    else if (attr is IniSection)
                    {
                        pb.HasPropertyName((attr as IniSection)?.Name);
                    }
                    else if (attr is IniArray)
                    {
                        var arry = attr as IniArray;
                        pb.HasArray(arry.Name, arry.BaseIndex);
                    }
                }
            }

            var pps = model.PropertyBuilds.Where(x => x.Value.Ignore == false);

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
        //[IniArray(Name = "Test")]
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
