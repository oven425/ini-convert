using QSoft.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp_IniT
{
    public interface IniString
    {
        string WriteToString(object obj);
    }

    public class IniModelSetting
    {
        public string Annotation { set; get; }
        public string SectionName { set; get; }
    }

    public class IniModelSetting<T>: IniModelSetting
    {
        public IniModelSetting()
        {
            this.SectionName = typeof(T).Name;
        }
    }

    public class IniModelBuilder<T> : IniString where T:class
    {
        Dictionary<string, PropertyInfo> m_Items;
        Dictionary<string, IniString> m_Setions = new Dictionary<string, IniString>();
        public Dictionary<string, IniPropertyBuilder> PropertyBuilds { private set; get; } = new Dictionary<string, IniPropertyBuilder>();
        List<IniString> m_IniStrings = new List<IniString>();
        public IniModelBuilder()
        {
            this.m_Items = typeof(T).GetProperties().ToDictionary(x => x.Name);
        }

        Action<IniModelSetting> m_SettingAction;
        public IniModelBuilder<T> useSetting(Action<IniModelSetting> action)
        {
            this.m_SettingAction = action;
            return this;
        }

        public IniPropertyBuilder<TProperty> Property<TProperty>(Expression<Func<T, TProperty>> func) 
        {
            var aa = func.Body as MemberExpression;
            if (aa.Expression.Type == typeof(T))
            {
                if (m_Items.ContainsKey(aa.Member.Name) == true)
                {
                    var pb = new IniPropertyBuilder<TProperty>(aa.Member as PropertyInfo);
                    this.PropertyBuilds[aa.Member.Name] = pb;
                    return pb;
                }
            }
            return null;
        }

        public IniModelBuilder<TProperty> Section<TProperty>(Expression<Func<T, TProperty>> func) where TProperty : class
        {
            var aa = func.Body as MemberExpression;
            if (aa.Expression.Type == typeof(T))
            {
                if (this.m_Setions.ContainsKey(aa.Member.Name) == false)
                {

                    var pb = new IniModelBuilder<TProperty>();
                    this.m_Setions.Add(aa.Member.Name, pb);
                    this.m_IniStrings.Add(pb);
                    return pb;
                }
                else
                {
                    var o1 = this.m_Setions[aa.Member.Name];
                    var o2 = o1 as IniModelBuilder<TProperty>;
                    return o2;
                }
            }
            return null;
        }

        public IniPropertyBuilder Property(string name)
        {
            if (this.m_Items.ContainsKey(name) == true)
            {
                var typedef = typeof(IniPropertyBuilder<>).MakeGenericType(this.m_Items[name].PropertyType);
                var pb =Activator.CreateInstance(typedef, this.m_Items[name]) as IniPropertyBuilder;
                this.PropertyBuilds[name] = pb;
                return pb;
            }
            return null;
        }

        public IniPropertyBuilder<TProperty> Property<TProperty>(string name)
        {
            if (this.m_Items.ContainsKey(name) == true)
            {
                var pb = new IniPropertyBuilder<TProperty>(this.m_Items[name]);
                this.PropertyBuilds[name] = pb;
                return pb;
            }
            return null;
        }

        public string WriteToString(object obj)
        {
            StringBuilder strb = new StringBuilder();
            IniModelSetting<T> setionsetting = new IniModelSetting<T>();

            this.m_SettingAction?.Invoke(setionsetting);
            if(string.IsNullOrEmpty(setionsetting.Annotation)==false)
            {
                StringReader sr = new StringReader(setionsetting.Annotation);
                while(true)
                {
                    var line = sr.ReadLine();
                    if(string.IsNullOrEmpty(line)==false)
                    {
                        strb.AppendLine($";{line}");
                    }
                    if(line == null)
                    {
                        break;
                    }
                }
            }
            strb.AppendLine($"[{setionsetting.SectionName}]");
            foreach (var item in this.m_Items)
            {
                var typecode = Type.GetTypeCode(item.Value.PropertyType);
                if(item.Value.PropertyType.IsGenericType==true && item.Value.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    typecode = Type.GetTypeCode(item.Value.PropertyType.GetGenericArguments()[0]);
                }
                if(typecode != TypeCode.Object)
                {
                    var oo = item.Value.GetValue(obj, null);
                    if (this.PropertyBuilds.ContainsKey(item.Key) == true)
                    {
                        var itemstr = this.PropertyBuilds[item.Key].WriteToString(oo);
                        strb.AppendLine(itemstr);
                    }
                    else
                    {
                        strb.AppendLine($"{item.Key}={oo}");
                    }
                }
            }
            
            foreach(var model in this.m_Setions)
            {
                var oo = this.m_Items[model.Key].GetValue(obj, null);
                var itemstr = model.Value.WriteToString(oo);
                strb.AppendLine(itemstr);
            }

            return strb.ToString();
        }
    }

    //https://github.com/dotnet/efcore/blob/main/src/EFCore/Metadata/Builders/PropertyBuilder.cs
    public class IniPropertyBuilder : IniString
    {
        public string Annotation { protected set; get; }
        public PropertyInfo Property { protected set; get; }
        public string Name { set; get; }
        public bool Ignore { protected set; get; }
        public int BaseIndex { protected set; get; }
        public IniPropertyBuilder()
        {

        }
        public IniPropertyBuilder(PropertyInfo property)
        {
            this.Property = property;
            this.Name = this.Property?.Name;
        }
        public IniPropertyBuilder HasPropertyName(string data)
        {
            this.Name = data;
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
            this.Name = name;
            this.BaseIndex = baseindex;
            return this;
        }


        public IniPropertyBuilder HasConvert(Func<object, object> write, Func<object, object> read)
        {
            this.m_ConvertTo = write;
            this.m_ConvertBack = read;
            return this;
        }

        public string WriteToString(object obj)
        {
            if(this.Ignore == true)
            {
                return "";
            }
            StringBuilder strb = new StringBuilder();
            if(string.IsNullOrEmpty(this.Annotation)==false)
            {
                strb.AppendLine($";{this.Annotation}");
            }
            strb.Append($"{this.Name}={obj}");
            return strb.ToString();
        }

        protected Func<object, object> m_ConvertTo;
        protected Func<object, object> m_ConvertBack;
    }

    public class IniPropertyBuilder<T> : IniPropertyBuilder
    {
        public IniPropertyBuilder(PropertyInfo property)
            : base(property)
        {

        }
        Delegate m_ConvertBack;
        Delegate m_Convert;

        public IniPropertyBuilder<T> HasConvert<T1>(Expression<Func<T, T1>> convert, Expression<Func<T1, T>> convertback)
        {
            this.m_Convert = convert.Compile();
            this.m_ConvertBack = convertback.Compile();

            return this;
        }
    }



    public class Section
    {
        public string Annotation { set; get; }
        public string Name { set; get; }
        public Dictionary<string, KeyValue> KeyValues { set; get; } = new Dictionary<string, KeyValue>();

        public static Dictionary<string, Section> Parse(string data)
        {
            Dictionary<string, Section> sections = new Dictionary<string, Section>();
            StringReader sr = new StringReader(data);
            while(true)
            {
                var line = sr.ReadLine();
                if(string.IsNullOrEmpty(line) == false)
                {
                    Regex regex_annotation = new Regex(";");
                }
                else
                {
                    break;
                }
            }
            return sections;
        }
    }

    public class KeyValue
    {
        public string Annotation { set; get; }
        public string Key { set; get; }
        public string Value { set; get; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Regex regex_annotation = new Regex(";(?<word>)");
            var regex_match = regex_annotation.Match(";aaaacccc");
            File.ReadAllText("setting.ini");
            IniModelBuilder<Setting> model = new IniModelBuilder<Setting>();
            model.useSetting(x =>
                {
                    x.Annotation = "123";
                    x.SectionName = "QQSeting";
                });
            
            model.Property(x => x.Port)
                .HasConvert(x => $"{x:X}", x => int.Parse(x))
                .HasAnnotation("Port default is 3333");
            model.Section(x => x.FTP)
                .useSetting(x =>
                    {
                        x.Annotation = "save source code";
                        x.SectionName = "FTP_AA";
                    });
            model.Section(x => x.FTP).Property(x => x.IP).HasAnnotation("FTP_IP");
            model.Section(x => x.FTP).Property(x => x.Port).HasAnnotation("FTP_PORT");
            model.Section(x => x.FTP).Property(x => x.Account).HasIgnore();
            model.Section(x => x.FTP).Property(x => x.Password).HasIgnore();
            //foreach (var pp in typeof(Setting).GetProperties())
            //{
            //    model.Property(x => x.Port).HasConvert(x => x.ToString(), x => int.Parse(x));
            //    var pb = model.Property(pp.Name);
            //    pb.HasConvert(x => x.ToString(), x => int.Parse(x.ToString()));
            //    var attrs = pp.GetCustomAttributes(true);
            //    foreach (var attr in attrs)
            //    {
            //        if (attr is IniIgnore)
            //        {
            //            pb.HasIgnore();
            //        }
            //        else if (attr is IniAnnotation)
            //        {
            //            pb.HasAnnotation((attr as IniAnnotation)?.Annotation);
            //        }
            //        else if (attr is IniSection)
            //        {
            //            pb.HasPropertyName((attr as IniSection)?.Name);
            //        }
            //        else if (attr is IniArray)
            //        {
            //            var arry = attr as IniArray;
            //            pb.HasArray(arry.Name, arry.BaseIndex);
            //        }
            //    }
            //}



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
            setting.FTP = new RemoteSetting() { IP = "192.168.1.1", Port = 8088, Account = "amind", Password = "admin1" };
            var pps = model.PropertyBuilds.Where(x => x.Value.Ignore == false);
            var inistr = model.WriteToString(setting);
            File.WriteAllText("setting.ini", inistr);
            StringBuilder strb = new StringBuilder();
            foreach (var pp in pps)
            {
                pp.Value.Property.GetValue(setting, null);
            }

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
        public int? Count { set; get; } = 100;
    [IniAnnotation(Annotation = "127.0.0.1 is default")]
    [IniAnnotation(Annotation = "Please check it")]
    public string IP { set; get; }
    public int Port { set; get; }
    public RemoteSetting FTP { set; get; }
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
