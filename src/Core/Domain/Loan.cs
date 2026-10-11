namespace Core.Domain;

public sealed class Loan
{
    public string Id { get; }
    public BookCopy Copy { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    public DateTime? ReturnedOn { get; private set; }

    public bool IsClosed => ReturnedOn.HasValue;

    private Loan(string id, BookCopy copy, string readerId, DateTime issuedOn, DateTime? returnedOn = null)
    {
        Id = id;
        Copy = copy;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        ReturnedOn = returnedOn;
    }

    // Фабричний метод відкриття видачі: автоматично переводить примірник у стан IsIssued
    public static Loan Open(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор видачі обов'язковий", nameof(id));

        ArgumentNullException.ThrowIfNull(copy);

        if (string.IsNullOrWhiteSpace(readerId))
            throw new ArgumentException("Ідентифікатор читача не може бути порожнім", nameof(readerId));

        // Викликаємо доменну операцію примірника (якщо вже виданий — кинеться InvalidOperationException)
        copy.Issue();

        return new Loan(id.Trim(), copy, readerId.Trim(), issuedOn);
    }

    // Бізнес-операція закриття видачі (інваріанти 5, 6)
    public void Close(DateTime returnedOn)
    {
        if (IsClosed)
            throw new InvalidOperationException($"Видача '{Id}' уже закрита раніше");

        if (returnedOn < IssuedOn)
            throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn,
                $"Дата повернення ({returnedOn:d}) не може передувати даті видачі ({IssuedOn:d})");

        ReturnedOn = returnedOn;
        Copy.Return();
    }

    public override string ToString() =>
        $"Видача {Id}: Читач {ReaderId} -> {Copy.Title} [Видано: {IssuedOn:d}, Закрито: {(IsClosed ? ReturnedOn?.ToString("d") : "Ні")}]";
}
