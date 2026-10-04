using System.Globalization;
using System.Text;
using Core.Dto;

namespace Core.Import;

public static class BookCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<BookDto> Load(string path)
    {
        var items = new List<BookDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            // Пропускаємо порожні рядки та коментарі
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            // Пропускаємо рядок заголовків (id;isbn;title;year;author)
            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<BookDto>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            // Патерн властивостей: замало колонок
            { Length: < 4 } => new ParseFailed($"очікую щонайменше 4 колонки, отримав {parts.Length}"),

            // Константний патерн списку: перевірка порожніх обов'язкових полів
            [_, "", _, ..] or [_, _, "", ..] => new ParseFailed("ISBN або назва не можуть бути порожніми"),

            // Охоронна умова when: валідація року за допомогою TryParse та меж
            [_, _, _, var year, ..] when !int.TryParse(year, NumberStyles.Integer, CultureInfo.InvariantCulture, out int y) || y < 1450 || y > DateTime.Now.Year
                => new ParseFailed($"рік '{year}' не є коректним роком видання (1450..{DateTime.Now.Year})"),

            // Патерн списку: 4 колонки (без автора)
            [var id, var isbn, var title, var year]
                => new ParseOk(new BookDto(id, isbn, title, int.Parse(year, CultureInfo.InvariantCulture))),

            // Патерн списку: 5 колонок (з автором)
            [var id, var isbn, var title, var year, var author]
                => new ParseOk(new BookDto(id, isbn, title, int.Parse(year, CultureInfo.InvariantCulture), string.IsNullOrWhiteSpace(author) ? null : author)),

            // Універсальна гілка на випадок зайвих колонок
            _ => new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(BookDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}
