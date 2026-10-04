using MyApp.Api.Infrastructure.Repositories.Common;

namespace MyApp.Api.Repositories.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken ct = default);
}
