// See https://aka.ms/new-console-template for more information
using QSoft.Ini;
Console.WriteLine("Hello, World!");



Console.ReadLine();

[IniSection]
public class Address
{
    [IniSectionKey]
    public string IP { set; get; }
    public int Port { set; get; }
    [IniIgnore]
    public string IP1 { set; get; }
}
