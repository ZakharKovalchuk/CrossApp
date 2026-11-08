using System.Text;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Ініціалізація сховища (Composition Root)
bool useFile = args.Contains("--file");
string dataPath = Path.Combine("data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Books());

var service = new LendingService(store);

// Якщо передано підкоманду звіту — виконуємо звіт, інакше запускаємо демо відкладеного виконання
var command = args.FirstOrDefault(a => !a.StartsWith("--"));

if (string.IsNullOrEmpty(command))
{
    Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");
    Console.WriteLine("--- Демонстрація відкладеного виконання (Deferred Execution) ---");

    var source = service.All().ToList();
    var query = source.Where(b => b.Year >= 2015); // Запит побудовано, але ще НЕ виконано

    Console.WriteLine($"Книг після 2015 року до змін: {query.Count()}");

    // Змінюємо джерело даних
    source.Add(BookCopy.Create("BC-TEMP", "978-0000000000", "New Framework in Action", 2024));
    Console.WriteLine($"Книг після 2015 року після source.Add: {query.Count()} (число змінилося!)");

    // Матеріалізація результату через ToList()
    var snapshot = query.ToList();
    source.Clear();

    Console.WriteLine($"Джерело очищено: source.Count = {source.Count}");
    Console.WriteLine($"У збереженому знімку snapshot.Count = {snapshot.Count} (дані зафіксовані)");

    Console.WriteLine("\nПідказка: використовуйте підкоманди для звітів:");
    Console.WriteLine("  dotnet run --project src/Cli -- report-top");
    Console.WriteLine("  dotnet run --project src/Cli -- report-groups");
    Console.WriteLine("  dotnet run --project src/Cli -- report-sum");
    Console.WriteLine("  dotnet run --project src/Cli -- report-issued");
    return;
}

var items = service.All();

switch (command)
{
    case "report-top":
        Console.WriteLine("=== ТОП-5 найновіших видань ===");
        Print(ReportService.TopRecent(items, 5));
        break;

    case "report-issued":
        Console.WriteLine("=== Видані примірники книги ===");
        Print(ReportService.IssuedCopies(items));
        break;

    case "report-groups":
        Console.WriteLine("=== Розподіл видань за десятиліттями ===");
        foreach (var g in ReportService.ByDecade(items))
        {
            Console.WriteLine($"  {g.Key,-10} позицій: {g.Count,3}");
        }
        break;

    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine("=== Загальна аналітика каталогу ===");
        Console.WriteLine($"  Всього позицій: {t.Items}, Середній рік: {t.AverageYear:F1}, Найновіша: {t.NewestYear}, Найстаріша: {t.OldestYear}");
        break;

    default:
        Console.WriteLine($"Невідома команда '{command}'. Доступні звіти:");
        Console.WriteLine("  report-top | report-issued | report-groups | report-sum");
        break;
}

static void Print(IReadOnlyList<BookReportLine> lines)
{
    if (lines.Count == 0)
    {
        Console.WriteLine("  (порожньо)");
        return;
    }

    foreach (var l in lines)
    {
        string status = l.IsIssued ? "Видано" : "В наявності";
        Console.WriteLine($"  {l.Id,-10} {l.Year,-6} {status,-14} {l.Title}");
    }
}
