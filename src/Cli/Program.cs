using System.Text;
using Core.Domain;

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

// 1. Порожній ISBN при створенні
TryDo("Створення з порожнім ISBN", () => 
    BookCopy.Create("BC-002", "", "Refactoring", 2018));

// 2. Некоректний рік видання
TryDo("Некоректний рік видання (3000)", () => 
    BookCopy.Create("BC-003", "978-1111", "Future Book", 3000));

// 3. Повторна видача вже виданого примірника
var busyCopy = BookCopy.Create("BC-004", "978-2222", "Design Patterns", 1994);
busyCopy.Issue();
TryDo("Повторна видача зайнятого примірника", () => 
    busyCopy.Issue());

// 4. Повернення примірника, який не був виданий
var freeCopy = BookCopy.Create("BC-005", "978-3333", "C# in Depth", 2019);
TryDo("Повернення невиданого примірника", () => 
    freeCopy.Return());

// 5. Закриття видачі датою, яка раніше за дату видачі
var testCopy = BookCopy.Create("BC-006", "978-4444", "Algorithms", 2011);
var testLoan = Loan.Open("L-102", testCopy, "READER-07", new DateTime(2026, 10, 5));
TryDo("Дата повернення раніша за дату видачі", () => 
    testLoan.Close(new DateTime(2026, 10, 1)));

// 6. Повторне закриття вже закритої видачі
testLoan.Close(new DateTime(2026, 10, 8));
TryDo("Повторне закриття закритої видачі", () => 
    testLoan.Close(new DateTime(2026, 10, 9)));

Console.WriteLine();
Console.WriteLine($"Фінальний стан зайнятого примірника: {busyCopy}");

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
