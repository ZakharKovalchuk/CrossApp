using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public BookCopy AddBook(string isbn, string title, int year)
    {
        // Генеруємо стабільний короткий унікальний ідентифікатор
        string id = $"BC-{Guid.NewGuid():N}"[..8].ToUpperInvariant();
        var copy = BookCopy.Create(id, isbn, title, year);
        _store.Add(copy);
        return copy;
    }

    public void IssueCopy(string copyId)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id='{copyId}' не знайдено.");

        copy.Issue(); // Доменний метод з тижня 4
        _store.Update(copy);
    }

    public void ReturnCopy(string copyId)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id='{copyId}' не знайдено.");

        copy.Return(); // Доменний метод з тижня 4
        _store.Update(copy);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();

    public BookCopy? Find(string copyId) => _store.GetById(copyId);
    
    // Додатковий метод сервісу для гнучкого пошуку
    public IReadOnlyList<BookCopy> Search(Func<BookCopy, bool> predicate) => _store.Find(predicate);
}
