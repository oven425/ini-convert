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
QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();
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
QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();
ini.Deserialize(inifile1, "test1.ini");
```

## Legacy
### write data to ini file
```csharp
QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();    
ini.Write("test", "bool", true, "test.ini");
ini.Write("test", "int", int.MaxValue, "test.ini");
ini.Write("test", "string", "write ini", "test.ini");
ini.Write("test", "Now", DateTime.Now, "test.ini");
ini.Write("test", "TimeSpan", new TimeSpan(1, 2, 3, 4), "test.ini");
```

```ini
[test]
bool=True
int=2147483647
string=write ini
Now=2020-12-28 15:15:12.561
TimeSpan=1.02:03:04
```
### read data from ini file
```csharp
QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();  
bool a = ini.Read<bool>("test", "bool", "test.ini");
int b = ini.Read<int>("test", "int", "test.ini");
string c = ini.Read<string>("test", "string", "test.ini");
DateTime d = ini.Read<DateTime>("test", "Now", "test.ini");
TimeSpan e = ini.Read<TimeSpan>("test", "TimeSpan", "test.ini");
```
## Advanced
If write type is Class,List or Array, 
auto transform object to xml and write ini

write sample code
```csharp
QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();  
ini.Write("test", "List", new List<int>() { 9, 8, 7 }, "test.ini");
ini.Write("test", "Array", new int[] { 1, 2, 3 }, "test.ini");
ini.Write("test", "CTest", new CTest() { A = "答案A", B = 101, Test1 = new CTest_1() { A1 = "答案B", B1 = 202 } }, "test.ini");
```

```ini
[test]
List=<?xml version="1.0" encoding="utf-8"?><ArrayOfInt xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><int>9</int><int>8</int><int>7</int></ArrayOfInt>
Array=<?xml version="1.0" encoding="utf-8"?><ArrayOfInt xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><int>1</int><int>2</int><int>3</int></ArrayOfInt>
CTest=<?xml version="1.0" encoding="utf-8"?><CTest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><A>答案A</A><B>101</B><Test1><A1>答案B</A1><B1>202</B1></Test1></CTest>
```

read sample code
```csharp
QSoft.Ini.IniSerializer ini = new QSoft.Ini.IniSerializer();  
List<int> f = ini.Read<List<int>>("test", "List", "test.ini");
int[] g = ini.Read<int[]>("test", "Array", "test.ini");
CTest h = ini.Read<CTest>("test", "CTest", "test.ini");
```
