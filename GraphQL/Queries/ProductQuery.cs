using HotChocolate.Authorization;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Api.GraphQL.Queries;

public sealed class ProductQuery
{
    [Authorize]
    public Task<List<Product>> GetProductsAsync(IProductRepository repository, CancellationToken ct) =>
        repository.GetAllAsync(ct);

    [Authorize]
    public Task<Product?> GetProductByIdAsync(string id, IProductRepository repository, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Product ID cannot be null or whitespace.", nameof(id));

        return repository.GetByIdAsync(id, ct);
    }

    [Authorize]
    public Task<List<Product>> GetProductsByIdsAsync(List<string> ids, IProductRepository repository, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return repository.GetByKeysAsync(ids, ct);
    }

    [Authorize]
    public Task<List<Product>> GetProductsByCategoryIdAsync(string categoryId, IProductRepository repository, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(categoryId))
            throw new ArgumentException("Category ID cannot be null or whitespace.", nameof(categoryId));

        return repository.GetProductsByCategoryIdAsync(categoryId, ct);
    }
}
