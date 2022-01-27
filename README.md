### Simple example
```c#
Setting setting = new Setting()
{
    IP = "127.0.0.1",
    Port = 88
};
string ini_str = IniConvert.SerializeObject(setting);
/*
[Setting]
IP=127.0.0.1
Port=88
*/
var deserialize = IniConvert.DeserializeObject<Setting>(ini_str);
```
[More](https://github.com/oven425/QSoft.Ini/wiki/Quick-Start)