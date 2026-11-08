using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IReadOnlyList<BookCopy> Books() =>
    [
        BookCopy.Create("BC-101", "978-0132350884", "Clean Code", 2008),
        BookCopy.Create("BC-102", "978-0201633610", "Design Patterns", 1994),
        BookCopy.Create("BC-103", "978-0134494166", "Clean Architecture", 2017),
        BookCopy.Create("BC-104", "978-0321125217", "Domain-Driven Design", 2003),
        BookCopy.Create("BC-105", "978-0596007126", "Head First Design Patterns", 2004),
        BookCopy.Create("BC-106", "978-1491950357", "Designing Data-Intensive Apps", 2017),
        BookCopy.Create("BC-107", "978-0131103627", "The C Programming Language", 1988),
        BookCopy.Create("BC-108", "978-0321573513", "Algorithms 4th Edition", 2011),
        BookCopy.Create("BC-109", "978-1449373320", "Designing Fine-Grained Systems", 2015),
        BookCopy.Create("BC-110", "978-0137081073", "The Clean Coder", 2011),
        BookCopy.Create("BC-111", "978-0201485677", "Refactoring", 1999),
        BookCopy.Create("BC-112", "978-0262033848", "Introduction to Algorithms", 2009),
        BookCopy.Create("BC-113", "978-1492040347", "Enterprise Integration Patterns", 2003),
        BookCopy.Create("BC-114", "978-1617294549", "Microservices Patterns", 2018),
        BookCopy.Create("BC-115", "978-0135974445", "Agile Principles, Patterns, and Practices", 2002),
        BookCopy.Create("BC-116", "978-1492077213", "Concurrency in C# Cookbook", 2019)
    ];
}
