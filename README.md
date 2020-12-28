# ini-convert
ini serialization like json xml serialization
## ini serialize
### object to ini file
1. First defined class
```csharp
[IniSection(DefaultSection = "General")]
public class CSetting
{
    [QSoft.Ini.IniSectionKey(Section = "Auth", Key = "Account")]
    public string Name { set; get; }

    [QSoft.Ini.IniSectionKey(Section = "Auth", Key = "Password")]
    public string Password { set; get; }

    [QSoft.Ini.IniSectionKey(Ignore = true)]
    public int MaxCount { set; get; } = 100;

    [QSoft.Ini.IniSectionKey(Key = "Min")]
    public int MinCount { set; get; } = 10;
    
    [QSoft.Ini.IniSectionKey(Key = "Duration")]
    public TimeSpan Time { set; get; }
}
```
2. Serialize
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
[CSetting]
Min=1
Duration=10.09:08:07
```
3. Deserialize
```csharp
CSetting inifile1 = new CSetting();
ini.Serialize(inifile1, "test1.ini");
```
## Legacy
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


