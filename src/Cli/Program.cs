using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

Console.OutputEncoding = Encoding.UTF8;

var info = new
{
    Application = "CrossApp – практикум з крос-платформного програмування",
    Student = "Ковальчук Захар",
    OSDescription = RuntimeInformation.OSDescription,
    OSVersion = Environment.OSVersion.ToString(),
    ProcessArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
    ClrVersion = Environment.Version.ToString(),
    FrameworkDescription = RuntimeInformation.FrameworkDescription,
    BaseDirectory = AppContext.BaseDirectory,
    CurrentDirectory = Environment.CurrentDirectory,
    Domain = "Бібліотека (Book, BookCopy, Reader, Loan)"
};

if (args.Contains("--json"))
{
    Console.WriteLine(JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

Console.WriteLine(info.Application);
Console.WriteLine($"Студент: {info.Student}");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"ОС (OSDescription)  : {info.OSDescription}");
Console.WriteLine($"ОС (Environment)    : {info.OSVersion}");
Console.WriteLine($"Архітектура процесу : {info.ProcessArchitecture}");
Console.WriteLine($"Версія .NET (CLR)   : {info.ClrVersion}");
Console.WriteLine($"Runtime             : {info.FrameworkDescription}");
Console.WriteLine($"Каталог застосунку  : {info.BaseDirectory}");
Console.WriteLine($"Поточний каталог    : {info.CurrentDirectory}");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"Предметна область   : {info.Domain}");
