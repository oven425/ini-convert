# ini-convert
ini serialization like json xml serialization
## Baic usage

```csharp
IniConvert.IniSerializer ini = new IniConvert.IniSerializer();
Dictionary<string, object> dic = new Dictionary<string, object>();
dic.Add("TimeSpan", TimeSpan.FromDays(3.21));
dic.Add("bool_true", bool.TrueString);
dic.Add("Now", DateTime.Now);
dic.Add("string", "ini serialize");
```

```ini
[test]
TimeSpan=3.05:02:24
bool_true=True
Now=2020-12-23 15:02:44.341
```
