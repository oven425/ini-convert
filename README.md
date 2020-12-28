# ini-convert
ini serialization like json xml serialization
## ini serialize
### object to ini file
1. first defined class
```csharp
[IniSection(DefaultSection = "General")]
public class CSetting
{
    [IniSectionKey(Section = "Auth", Key = "Account")]
    public string Name { set; get; }

    [IniSectionKey(Section = "Auth", Key = "Password")]
    public string Password { set; get; }

    [IniSectionKey(Ignore = true)]
    public int MaxCount { set; get; } = 100;

    [IniSectionKey(Key = "MinCount")]
    public int MinCount { set; get; } = 10;
}
```
2. serialize
```csharp
CSetting inifile = new CSetting();
inifile.MaxCount = 1000;
inifile.MinCount = 1;
inifile.Name = "account";
inifile.Password = "password";
ini.Serialize(inifile, "test1.ini");
```

```ini
[Auth]
Account=account
Password=password
[General]
MinCount=1
```
## Basic usage
### object to ini file
```csharp
IniConvert.IniSerializer ini = new IniConvert.IniSerializer();
Dictionary<string, object> dic = new Dictionary<string, object>();
dic.Add("TimeSpan", TimeSpan.FromDays(3.21));
dic.Add("bool_true", bool.TrueString);
dic.Add("Now", DateTime.Now);
dic.Add("string", "ini serialize");
ini.Serialize("test", dic, "test.ini");
```

```ini
[test]
TimeSpan=3.05:02:24
bool_true=True
Now=2020-12-23 15:02:44.341
```
###  ini file to object
```csharp

```


