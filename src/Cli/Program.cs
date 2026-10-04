using System.Text;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = Encoding.UTF8;

// Отримуємо шлях із args або використовуємо data/sample.csv за замовчуванням
string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<BookDto> result = BookCsvImporter.Load(path);

Console.WriteLine("Результати імпорту каталогу книг:");
Console.WriteLine(new string('-', 72));
Console.WriteLine($"Завантажено успішно : {result.Items.Count}");
Console.WriteLine($"Пропущено з помилками: {result.Errors.Count}");
Console.WriteLine(new string('-', 72));

Console.WriteLine("Перші завантажені книги:");
foreach (BookDto book in result.Items.Take(5))
{
    string author = book.Author ?? "(автор не вказаний)";
    Console.WriteLine($" {book.Id,-6} | {book.Isbn,-17} | {book.Year,4} | {book.Title,-28} | {author}");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine(new string('-', 72));
    Console.WriteLine("Виявлені помилки у файлі:");
    foreach (string error in result.Errors)
    {
        Console.WriteLine($"  ! {error}");
    }
}

return 0;