using System.Text;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Composition Root
bool useFile = args.Contains("--file");
string dataPath = Path.Combine("data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Books());

var service = new LendingService(store);

Console.WriteLine($"=== Сховище: {store.GetType().Name} ===");
if (useFile)
{
    Console.WriteLine($"Шлях до файлу: {dataPath}");
}

// 1. Додавання нової книги
var newBook = service.AddBook("978-0131103627", "The C Programming Language", 1988);
Console.WriteLine($"Додано нову книгу: {newBook}");

// 2. Зміна стану
service.IssueCopy(newBook.Id);
Console.WriteLine($"Видано книгу: {service.Find(newBook.Id)}");

// 3. Виведення всіх записів
Console.WriteLine("\nПоточний каталог книг:");
foreach (var b in service.All())
{
    Console.WriteLine($" {b.Id,-10} {b.Isbn,-16} {b.Title,-32} {b.Year}  [{(b.IsIssued ? "Видано" : "В наявності")}]");
}

// 4. Демонстрація додаткового завдання: гнучкий пошук через Func<BookCopy, bool>
Console.WriteLine("\n=== ДОДАТКОВЕ ЗАВДАННЯ: Пошук за предикатом ===");
Console.WriteLine("Книги, видані після 2010 року:");
var modernBooks = service.Search(b => b.Year > 2010);
foreach (var b in modernBooks)
{
    Console.WriteLine($" -> {b.Title} ({b.Year})");
}

Console.WriteLine("Книги зі словом 'Clean' у назві:");
var cleanBooks = service.Search(b => b.Title.Contains("Clean", StringComparison.OrdinalIgnoreCase));
foreach (var b in cleanBooks)
{
    Console.WriteLine($" -> {b.Title}");
}

// 5. Сценарії відмов
Console.WriteLine("\n=== Сценарії відмов ===");

TryDo("Спроба видати неіснуючий примірник", () =>
    service.IssueCopy("UNKNOWN-ID"));

TryDo("Спроба додати дублікат примірника напряму в сховище", () =>
    store.Add(newBook));

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" [FAIL] {title}: виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [OK]   {title}: {ex.GetType().Name} -> {ex.Message}");
    }
}
