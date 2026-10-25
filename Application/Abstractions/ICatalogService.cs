using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions;

public interface ICatalogService
{
    Task<Category> AddCategoryAsync(string name, CancellationToken ct = default);
    Task<Category> UpdateCategoryAsync(string id, string name, CancellationToken ct = default);
    Task<Category> DeleteCategoryAsync(string id, CancellationToken ct = default);
    Task<Product> AddProductAsync(string name, decimal price, string categoryId, int stock, CancellationToken ct = default);
    Task<Product> UpdateProductAsync(string id, string? name, decimal? price, string? categoryId, int? stock, CancellationToken ct = default);
    Task<Product> DeleteProductAsync(string id, CancellationToken ct = default);
}