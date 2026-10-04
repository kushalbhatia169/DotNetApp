using MyApp.Api.Infrastructure.Repositories.Common;

namespace MyApp.Api.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    // Sirf Product-specific new calls yahan define hongi
    Task<List<Product>> GetProductsByCategoryIdAsync(string categoryId, CancellationToken ct = default);
}