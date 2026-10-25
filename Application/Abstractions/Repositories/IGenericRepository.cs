using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task<bool> ExistsAsync(string id, CancellationToken ct = default);
    Task<List<T>> GetByKeysAsync(List<string> keys, CancellationToken ct = default);
    Task UpdateAll(List<T> entities, CancellationToken ct = default);
    Task DeleteAll(List<string> ids, CancellationToken ct = default);
}