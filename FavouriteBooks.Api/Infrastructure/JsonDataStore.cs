using System.Collections.Concurrent;
using System.Text.Json;

namespace FavouriteBooks.Api.Infrastructure;

public class JsonDataStore
{
    private readonly string _dataDirectory;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _fileLocks = new();

    public JsonDataStore(IWebHostEnvironment environment)
    {
        _dataDirectory = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(_dataDirectory);

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<List<T>> ReadCollectionAsync<T>(string fileName)
    {
        var path = GetFilePath(fileName);
        EnsureFileExists(path);

        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (stream.Length == 0)
        {
            return [];
        }

        var data = await JsonSerializer.DeserializeAsync<List<T>>(stream, _serializerOptions);
        return data ?? [];
    }

    public async Task WriteCollectionAsync<T>(string fileName, IEnumerable<T> items)
    {
        var path = GetFilePath(fileName);
        EnsureFileExists(path);

        var fileLock = _fileLocks.GetOrAdd(path, _ => new SemaphoreSlim(1, 1));
        await fileLock.WaitAsync();

        try
        {
            await using var stream = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(stream, items, _serializerOptions);
        }
        finally
        {
            fileLock.Release();
        }
    }

    public bool FileExists(string fileName)
    {
        var path = GetFilePath(fileName);
        return File.Exists(path);
    }

    public string GetFilePath(string fileName) => Path.Combine(_dataDirectory, fileName);

    private static void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "[]");
        }
    }
}
