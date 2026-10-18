using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Dto;

namespace Core.Storage;

public sealed class FileBookStore(string path) : IBookStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly Dictionary<string, BookCopy> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _path = Path.GetFullPath(path);
    private bool _loaded;

    private void EnsureLoaded()
    {
        if (_loaded) return;

        if (File.Exists(_path))
        {
            var json = File.ReadAllText(_path);
            var dtos = JsonSerializer.Deserialize<List<BookDto>>(json) ?? [];
            foreach (var dto in dtos)
            {
                var copy = BookCopy.FromDto(dto);
                _cache[copy.Id] = copy;
            }
        }

        _loaded = true;
    }

    private void Flush()
    {
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var dtos = _cache.Values.Select(p => p.ToDto()).ToList();
        File.WriteAllText(_path, JsonSerializer.Serialize(dtos, Options));
    }

    public IReadOnlyList<BookCopy> List()
    {
        EnsureLoaded();
        return _cache.Values.ToList();
    }

    public BookCopy? GetById(string id)
    {
        EnsureLoaded();
        return _cache.GetValueOrDefault(id);
    }

    public void Add(BookCopy item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();

        if (_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Примірник з id='{item.Id}' уже існує у файлі.");

        _cache.Add(item.Id, item);
        Flush();
    }

    public void Update(BookCopy item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();

        if (!_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Неможливо оновити: запис з id='{item.Id}' не знайдено.");

        _cache[item.Id] = item;
        Flush();
    }

    public bool Remove(string id)
    {
        EnsureLoaded();
        if (!_cache.Remove(id)) return false;
        Flush();
        return true;
    }
    // Реалізація пошуку за предикатом з кешу
    public IReadOnlyList<BookCopy> Find(Func<BookCopy, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        EnsureLoaded();
        return _cache.Values.Where(predicate).ToList();
    }
}
