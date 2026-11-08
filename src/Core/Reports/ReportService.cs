using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    // 1) Фільтр + сортування + проєкція в DTO
    public static IReadOnlyList<BookReportLine> IssuedCopies(IEnumerable<BookCopy> source) =>
        source.Where(b => b.IsIssued)
              .OrderBy(b => b.Year)
              .ThenBy(b => b.Title, StringComparer.Ordinal)
              .Select(b => new BookReportLine(b.Id, b.Title, b.Year, b.IsIssued))
              .ToList();

    // 2) Топ-N найновіших видань зі стабільним детермінованим сортуванням
    public static IReadOnlyList<BookReportLine> TopRecent(IEnumerable<BookCopy> source, int n) =>
        source.OrderByDescending(b => b.Year)
              .ThenBy(b => b.Title, StringComparer.Ordinal)
              .Take(n)
              .Select(b => new BookReportLine(b.Id, b.Title, b.Year, b.IsIssued))
              .ToList();

    // 3) Групування за десятиліттями або роками (GroupBy + агрегати всередині групи)
    public static IReadOnlyList<GroupSummary> ByDecade(IEnumerable<BookCopy> source) =>
        source.GroupBy(b => $"{b.Year / 10 * 10}-ті")
              .Select(g => new GroupSummary(g.Key, g.Count()))
              .OrderByDescending(s => s.Count)
              .ThenBy(s => s.Key, StringComparer.Ordinal)
              .ToList();

    // 4) Агрегати з безпечною обробкою порожнього джерела (без InvalidOperationException)
    public static (int Items, double AverageYear, int NewestYear, int OldestYear) Totals(IEnumerable<BookCopy> source)
    {
        var list = source.ToList();
        return list.Count == 0
            ? (0, 0d, 0, 0)
            : (list.Count, list.Average(b => b.Year), list.Max(b => b.Year), list.Min(b => b.Year));
    }
}
