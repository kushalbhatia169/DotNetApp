using HotChocolate.Authorization;
using MyApp.Api.GraphQL.Inputs;
using MyApp.Application.Abstractions;
using MyApp.Domain.Entities;

namespace MyApp.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public sealed class ProductMutation
{
    [Authorize(Roles = new[] { "Admin" })]
    public Task<Product> AddProductAsync(
        AddProductInput input,
        [Service] ICatalogService catalogService,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        return catalogService.AddProductAsync(input.Name, input.Price, input.CategoryId, input.Stock, ct);
    }

    [Authorize(Roles = new[] { "Admin" })]
    public Task<Product> UpdateProductAsync(
        string productId,
        UpdateProductInput input,
        [Service] ICatalogService catalogService,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        return catalogService.UpdateProductAsync(productId, input.Name, input.Price, input.CategoryId, input.Stock, ct);
    }

    [Authorize(Roles = new[] { "Admin" })]
    public Task<Product> DeleteProductAsync(
        string productId,
        [Service] ICatalogService catalogService,
        CancellationToken ct) =>
        catalogService.DeleteProductAsync(productId, ct);

    [Authorize(Roles = new[] { "Admin" })]
    public async Task<List<Product>> UpdateBulkProductAsync(
        List<string> productIds,
        UpdateProductInput input,
        [Service] ICatalogService catalogService,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Name) && input.Price == null && string.IsNullOrWhiteSpace(input.CategoryId) && input.Stock == null)
        {
            throw new ArgumentException("At least one field must be provided for update.", nameof(input));
        }
        var updatedProducts = new List<Product>();
        foreach (var productId in productIds)
        {
            var updatedProduct = await catalogService.UpdateProductAsync(productId, input.Name, input.Price, input.CategoryId, input.Stock, ct);
            updatedProducts.Add(updatedProduct);
        }
        return updatedProducts;
    }

    public async Task<List<Product>> DeleteBulkProductAsync(
        List<string> productIds,
        [Service] ICatalogService catalogService,
        CancellationToken ct)
    {
        var deletedProducts = new List<Product>();
        foreach (var productId in productIds)
        {
            var deletedProduct = await catalogService.DeleteProductAsync(productId, ct);
            deletedProducts.Add(deletedProduct);
        }
        return deletedProducts;
    }
}