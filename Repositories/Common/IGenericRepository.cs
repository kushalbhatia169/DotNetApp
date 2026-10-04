using System.Threading;

namespace MyApp.Api.Infrastructure.Repositories.Common;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    Task SaveAllAsync(List<T> entities, CancellationToken ct = default);
}
