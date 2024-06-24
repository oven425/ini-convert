// See https://aka.ms/new-console-template for more information
using System.Text.Json.Serialization;
using System.Text.Json;

Console.WriteLine("Hello, World!");
var s = "Hello World!";
// JsonTypeInfo<T>
Console.WriteLine(JsonSerializer.Serialize(s, MyJsonSerializerContext.Default.String));
// JsonSerializerContext
Console.WriteLine(JsonSerializer.Serialize(s, typeof(string), MyJsonSerializerContext.Default));
// JsonSerializerOptions
var jsonOptions = new JsonSerializerOptions
{
TypeInfoResolver = MyJsonSerializerContext.Default
};
// 以下兩種寫法可成功，但會有 RequiresUnreferencedCodeAttribute 警告，不推
//Console.WriteLine(JsonSerializer.Serialize(s, typeof(string), jsonOptions));
//Console.WriteLine(JsonSerializer.Serialize(s, jsonOptions));

var p = new Player
{
Name = "Jeffrey",
RegDate = DateTime.Today,
Score = 32767
};
var json = JsonSerializer.Serialize(p, MyJsonSerializerContext.Default.Player);
Console.WriteLine(json);
// 反序列化
var restored = JsonSerializer.Deserialize<Player>(json, MyJsonSerializerContext.Default.Player);
Console.WriteLine($"{restored.Name} == {p.Name}");

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Player))]
internal partial class MyJsonSerializerContext : JsonSerializerContext
{
}

public class Player
{
    public string Name { get; set; }
    public DateTime RegDate { get; set; }
    public int Score { get; set; }
}