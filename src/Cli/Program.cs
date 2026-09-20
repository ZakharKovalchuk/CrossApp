using System.Text;
using System.Text.Json;
using Core;

Console.OutputEncoding = Encoding.UTF8;

// Отримуємо дані виключно через бібліотеку Core
EnvironmentReport report = EnvironmentInfo.Collect();

// Підтримка JSON-виводу
if (args.Contains("--json"))
{
    Console.WriteLine(JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
    return;
}

Console.WriteLine("CrossApp – інформація про середовище (Лабораторна 2)");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"ОС                 : {report.OsDescription}");
Console.WriteLine($"Runtime            : {report.FrameworkDescription}");
Console.WriteLine($"Архітектура        : {report.ProcessArchitecture}");
Console.WriteLine($"RID (визначено)    : {report.DetectedRid}");
Console.WriteLine($"RID (від .NET)     : {report.ReportedRid}");
Console.WriteLine($"Каталог застосунку : {report.BaseDirectory}");
