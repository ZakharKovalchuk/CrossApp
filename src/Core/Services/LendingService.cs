using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    // Подія зміни стану бібліотеки
    public event EventHandler<LibraryChangedEventArgs>? Changed;

    private void OnChanged(string copyId, LibraryChangeKind kind, string? readerId = null, DateOnly? dueDate = null)
    {
        Changed?.Invoke(this, new LibraryChangedEventArgs(copyId, kind, readerId, dueDate));
    }

    public BookCopy AddBook(string isbn, string title, int year)
    {
        string id = $"BC-{Guid.NewGuid():N}"[..8].ToUpperInvariant();
        var copy = BookCopy.Create(id, isbn, title, year);
        _store.Add(copy);

        // Піднімаємо подію лише після успішного збереження
        OnChanged(copy.Id, LibraryChangeKind.BookAdded);
        return copy;
    }

    public void IssueCopy(string copyId, string readerId, DateOnly dueDate)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id='{copyId}' не знайдено.");

        copy.Issue(); // Перевірка інваріанту (тиждень 4)
        _store.Update(copy);

        // Сповіщення про видачу читачеві
        OnChanged(copy.Id, LibraryChangeKind.CopyIssued, readerId, dueDate);
    }

    public void ReturnCopy(string copyId)
    {
        var copy = _store.GetById(copyId)
            ?? throw new InvalidOperationException($"Примірник з id='{copyId}' не знайдено.");

        copy.Return(); // Перевірка інваріанту (тиждень 4)
        _store.Update(copy);

        // Сповіщення про повернення книги
        OnChanged(copy.Id, LibraryChangeKind.CopyReturned);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();

    public BookCopy? Find(string copyId) => _store.GetById(copyId);

    // Делегат Func для фільтрації
    public IReadOnlyList<BookCopy> FindAll(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}
