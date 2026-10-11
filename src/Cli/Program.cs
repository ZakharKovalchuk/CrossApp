using System.Text;
using Core.Domain;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== СЦЕНАРІЙ 1: УСПІШНА РОБОТА ДОМЕНУ ===");
var copy = BookCopy.Create("BC-001", "978-0132350884", "Clean Code", 2008);
Console.WriteLine($"Створено примірник : {copy}");

var loanDate = new DateTime(2026, 10, 1);
var loan = Loan.Open("L-101", copy, "READER-42", loanDate);
Console.WriteLine($"Відкрито видачу    : {loan}");
Console.WriteLine($"Стан примірника    : {copy}");

loan.Close(new DateTime(2026, 10, 10));
Console.WriteLine($"Закрито видачу     : {loan}");
Console.WriteLine($"Стан примірника    : {copy}");

Console.WriteLine();
Console.WriteLine("=== СЦЕНАРІЙ 2: ПЕРЕВІРКА ЗАХИСТУ ІНВАРІАНТІВ ===");

TryDo("Створення з порожнім ISBN", () => 
    BookCopy.Create("BC-002", "", "Refactoring", 2018));

TryDo("Некоректний рік видання (3000)", () => 
    BookCopy.Create("BC-003", "978-1111", "Future Book", 3000));

var busyCopy = BookCopy.Create("BC-004", "978-2222", "Design Patterns", 1994);
busyCopy.Issue();
TryDo("Повторна видача зайнятого примірника", () => 
    busyCopy.Issue());

var freeCopy = BookCopy.Create("BC-005", "978-3333", "C# in Depth", 2019);
TryDo("Повернення невиданого примірника", () => 
    freeCopy.Return());

var testCopy = BookCopy.Create("BC-006", "978-4444", "Algorithms", 2011);
var testLoan = Loan.Open("L-102", testCopy, "READER-07", new DateTime(2026, 10, 5));
TryDo("Дата повернення раніша за дату видачі", () => 
    testLoan.Close(new DateTime(2026, 10, 1)));

testLoan.Close(new DateTime(2026, 10, 8));
TryDo("Повторне закриття закритої видачі", () => 
    testLoan.Close(new DateTime(2026, 10, 9)));

Console.WriteLine();
Console.WriteLine("=== ДОДАТКОВІ ЗАВДАННЯ ===");

// 1. Демонстрація переходів станів через enum LoanStatus
Console.WriteLine("1. Перевірка невалідного переходу стану видачі:");
var overdueCopy = BookCopy.Create("BC-007", "978-5555", "CLR via C#", 2012);
var overdueLoan = Loan.Open("L-103", overdueCopy, "READER-11", new DateTime(2026, 9, 1));
overdueLoan.ChangeStatus(LoanStatus.Overdue);
Console.WriteLine($"  Переведено у прострочений стан: {overdueLoan}");
overdueLoan.Close(new DateTime(2026, 10, 11));
Console.WriteLine($"  Успішно закрито з простроченого: {overdueLoan}");
TryDo("Спроба перевести вже закриту видачу в Overdue", () => 
    overdueLoan.ChangeStatus(LoanStatus.Overdue));

// 2. Демонстрація перетворення ImportResult<BookDto> у доменні сутності
Console.WriteLine();
Console.WriteLine("2. Зв'язок із лабораторною 3 (ImportResult -> BookCopy):");
string samplePath = Path.Combine("data", "sample.csv");
if (File.Exists(samplePath))
{
    var importResult = BookCsvImporter.Load(samplePath);
    var (validCopies, allErrors) = BookCopy.FromImportResult(importResult);
    Console.WriteLine($"  Успішно створено доменних сутностей: {validCopies.Count}");
    Console.WriteLine($"  Загальна кількість помилок (CSV + Домен): {allErrors.Count}");
}

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" [FAIL] {title}: Виняток НЕ спрацював!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" [OK]   {title}: {ex.GetType().Name} -> {ex.Message}");
    }
}
