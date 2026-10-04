using System.Text.Json;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Path = System.IO.Path;

namespace MyApp.Api.Infrastructure.Repositories.Common;

public abstract class GenericJsonRepository<T>(string filePath) : IGenericRepository<T> where T : BaseEntity
{
    private readonly string _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            var jsonText = await File.ReadAllTextAsync(_filePath, ct);
            return JsonSerializer.Deserialize<List<T>>(jsonText) ?? new List<T>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var items = await GetAllAsync(ct);
        return items.FirstOrDefault(i => i.Id == id);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)    
    {
        var items = await GetAllAsync(ct);
        if (string.IsNullOrWhiteSpace(entity.Id)) entity.Id = Guid.NewGuid().ToString();
        items.Add(entity);
        await SaveAllAsync(items, ct);
    }

    public async Task SaveAllAsync(List<T> entities, CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        try
        {
            var json = JsonSerializer.Serialize(entities, new JsonSerializerOptions { WriteIndented = true });
            var dir = Path.GetDirectoryName(_filePath) ?? ".";
            var tempPath = Path.Combine(dir, Path.GetRandomFileName());
            await File.WriteAllTextAsync(tempPath, json, ct);

            if (File.Exists(_filePath))
                File.Replace(tempPath, _filePath, null);
            else
                File.Move(tempPath, _filePath);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
