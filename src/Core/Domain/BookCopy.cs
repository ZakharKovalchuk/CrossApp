using Core.Dto;

namespace Core.Domain;

public sealed class BookCopy
{
    public string Id { get; }
    public string Isbn { get; }
    public string Title { get; }
    public int Year { get; }
    public bool IsIssued { get; private set; }

    private BookCopy(string id, string isbn, string title, int year, bool isIssued)
    {
        Id = id;
        Isbn = isbn;
        Title = title;
        Year = year;
        IsIssued = isIssued;
    }

    // Фабричний метод для створення нового примірника (інваріанти 1, 2)
    public static BookCopy Create(string id, string isbn, string title, int year, bool isIssued = false)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор примірника не може бути порожнім", nameof(id));

        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN не може бути порожнім", nameof(isbn));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Назва книги не може бути порожньою", nameof(title));

        if (year < 1450 || year > DateTime.Now.Year)
            throw new ArgumentOutOfRangeException(nameof(year), year, $"Рік видання має бути в межах 1450..{DateTime.Now.Year}");

        return new BookCopy(id.Trim(), isbn.Trim().ToUpperInvariant(), title.Trim(), year, isIssued);
    }

    // Бізнес-операція видачі примірника (інваріант 3)
    public void Issue()
    {
        if (IsIssued)
            throw new InvalidOperationException($"Примірник '{Id}' ({Title}) вже виданий, повторна видача неможлива");

        IsIssued = true;
    }

    // Бізнес-операція повернення примірника (інваріант 4)
    public void Return()
    {
        if (!IsIssued)
            throw new InvalidOperationException($"Примірник '{Id}' ({Title}) не був виданий, повернення неможливе");

        IsIssued = false;
    }

    // Мапінг у DTO та відновлення з DTO через фабрику
    public BookDto ToDto() => new(Id, Isbn, Title, Year);

    public static BookCopy FromDto(BookDto dto) =>
        Create(dto.Id, dto.Isbn, dto.Title, dto.Year);

    public override string ToString() =>
        $"{Id} [{Isbn}] \"{Title}\" ({Year}) - Стан: {(IsIssued ? "Видано" : "В наявності")}";
}
