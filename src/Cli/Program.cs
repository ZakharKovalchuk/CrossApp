using System.Text;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Events;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// 1. Ініціалізація сховища та сервісу (Composition Root)
bool useFile = args.Contains("--file");
string dataPath = Path.Combine("data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Books());

var service = new LendingService(store);
Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");

// 2. Налаштування файлу журналу подій (logs/app.log)
string logDir = Path.Combine(AppContext.BaseDirectory, "logs");
string logPath = Path.Combine(logDir, "app.log");
Directory.CreateDirectory(logDir);

// Підписник 1: Локальний метод для логування в консоль та у файл
void OnLibraryChanged(object? sender, LibraryChangedEventArgs e)
{
    string details = e.Kind switch
    {
        LibraryChangeKind.BookAdded    => "Додано нову книгу",
        LibraryChangeKind.CopyIssued   => $"Видано читачу {e.ReaderId} до {e.DueDate:yyyy-MM-dd}",
        LibraryChangeKind.CopyReturned => "Повернено до фонду",
        _ => string.Empty
    };

    string line = $"{e.At:HH:mm:ss} {e.Kind,-12} {e.CopyId,-10} -> {details}";
    Console.WriteLine($" [LOG] {line}");
    File.AppendAllText(logPath, line + Environment.NewLine);
}

// Підписка через +=
service.Changed += OnLibraryChanged;

// Підписник 2: Лямбда-вираз із замиканням над лічильником changes
int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine("\n--- Крок 1: Дії з активними двома підписниками ---");
var book = service.AddBook("978-0131103627", "The C Programming Language", 1988);
service.IssueCopy(book.Id, "READER-42", DateOnly.FromDateTime(DateTime.Today.AddDays(14)));
service.ReturnCopy(book.Id);

Console.WriteLine($"Подій зафіксовано лічильником: {changes}"); // Очікується: 3

Console.WriteLine("\n--- Крок 2: Відписка OnLibraryChanged (-=) ---");
service.Changed -= OnLibraryChanged;

// Ця операція буде «тихою» для консолі/файлу, але лічильник збільшиться
service.IssueCopy(book.Id, "READER-99", DateOnly.FromDateTime(DateTime.Today.AddDays(7)));
Console.WriteLine($"Подій після відписки головного логера: {changes}"); // Очікується: 4

Console.WriteLine("\n--- Крок 3: Сценарій відмови (інваріант не пропускає дію) ---");
try
{
    // Спроба видати книгу, яка вже видана читачу READER-99
    service.IssueCopy(book.Id, "READER-FAIL", DateOnly.FromDateTime(DateTime.Today.AddDays(10)));
}
catch (Exception ex)
{
    Console.WriteLine($" [ERROR CATCHED] {ex.GetType().Name}: {ex.Message}");
}
// Перевірка, що лічильник не змінився, бо операція не вдалася
Console.WriteLine($"Кількість подій після помилки: {changes}"); // Залишається 4

Console.WriteLine("\n--- Крок 4: Використання делегата Func<BookCopy, bool> (FindAll) ---");
Console.WriteLine("Книги до 2000 року видання:");
var classicBooks = service.FindAll(b => b.Year < 2000);
foreach (var b in classicBooks)
{
    Console.WriteLine($" * {b.Id}: \"{b.Title}\" ({b.Year})");
}
