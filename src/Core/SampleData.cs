using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<BookCopy> Books() =>
    [
        BookCopy.Create("BC-101", "978-0132350884", "Clean Code", 2008),
        BookCopy.Create("BC-102", "978-0201633610", "Design Patterns", 1994),
        BookCopy.Create("BC-103", "978-0134494166", "Clean Architecture", 2017),
        BookCopy.Create("BC-104", "978-1491987650", "Kafka: The Definitive Guide", 2017),
        BookCopy.Create("BC-105", "978-0321125217", "Domain-Driven Design", 2003)
    ];
}
