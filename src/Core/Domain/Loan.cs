namespace Core.Domain;

public sealed class Loan
{
    public string Id { get; }
    public BookCopy Copy { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    public DateTime? ReturnedOn { get; private set; }
    public LoanStatus Status { get; private set; }

    public bool IsClosed => Status == LoanStatus.Closed;

    private Loan(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        Id = id;
        Copy = copy;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        Status = LoanStatus.Open;
    }

    public static Loan Open(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Ідентифікатор видачі обов'язковий", nameof(id));

        ArgumentNullException.ThrowIfNull(copy);

        if (string.IsNullOrWhiteSpace(readerId))
            throw new ArgumentException("Ідентифікатор читача не може бути порожнім", nameof(readerId));

        copy.Issue();
        return new Loan(id.Trim(), copy, readerId.Trim(), issuedOn);
    }

    // Додаткове завдання 3: валідація переходу станів через switch expression
    public void ChangeStatus(LoanStatus newStatus)
    {
        Status = (Status, newStatus) switch
        {
            (LoanStatus.Open, LoanStatus.Overdue) => LoanStatus.Overdue,
            (LoanStatus.Overdue, LoanStatus.Closed) => LoanStatus.Closed,
            (LoanStatus.Open, LoanStatus.Closed) => LoanStatus.Closed,
            (var current, var target) when current == target => current,
            _ => throw new InvalidOperationException($"Неприпустимий перехід стану видачі: з '{Status}' у '{newStatus}'")
        };
    }

    public void Close(DateTime returnedOn)
    {
        if (IsClosed)
            throw new InvalidOperationException($"Видача '{Id}' уже закрита раніше");

        if (returnedOn < IssuedOn)
            throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn,
                $"Дата повернення ({returnedOn:d}) не може передувати даті видачі ({IssuedOn:d})");

        ChangeStatus(LoanStatus.Closed);
        ReturnedOn = returnedOn;
        Copy.Return();
    }

    public override string ToString() =>
        $"Видача {Id}: Читач {ReaderId} -> {Copy.Title} [Статус: {Status}, Видано: {IssuedOn:d}, Закрито: {(ReturnedOn.HasValue ? ReturnedOn.Value.ToString("d") : "Ні")}]";
}
