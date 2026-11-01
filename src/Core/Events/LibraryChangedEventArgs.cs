namespace Core.Events;

public sealed class LibraryChangedEventArgs(
    string copyId,
    LibraryChangeKind kind,
    string? readerId = null,
    DateOnly? dueDate = null) : EventArgs
{
    public string CopyId { get; } = copyId;
    public LibraryChangeKind Kind { get; } = kind;
    public string? ReaderId { get; } = readerId;
    public DateOnly? DueDate { get; } = dueDate;
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}
