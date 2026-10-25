using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<bool> NameExistsAsync(string name, string? exceptId = null, CancellationToken ct = default);
    Task<List<Product>> GetProductsByCategoryIdAsync(string categoryId, CancellationToken ct = default);
}