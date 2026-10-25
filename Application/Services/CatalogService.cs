using MyApp.Application.Abstractions;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Application.Services;

public sealed class CatalogService(ICategoryRepository categoryRepository, IProductRepository productRepository) : ICatalogService
{
    public async Task<Category> AddCategoryAsync(string name, CancellationToken ct = default)
    {
        var normalizedName = NormalizeRequiredName(name, "Category");
        if (await categoryRepository.NameExistsAsync(normalizedName, ct: ct))
        {
            throw new InvalidOperationException($"A category with the name '{normalizedName}' already exists.");
        }

        var category = new Category { Name = normalizedName };
        await categoryRepository.AddAsync(category, ct);
        return category;
    }

    public async Task<Category> UpdateCategoryAsync(string id, string name, CancellationToken ct = default)
    {
        var categoryId = NormalizeRequiredName(id, "Category ID");
        var category = await categoryRepository.GetByIdAsync(categoryId, ct)
            ?? throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
        var normalizedName = NormalizeRequiredName(name, "Category");
        if (await categoryRepository.NameExistsAsync(normalizedName, categoryId, ct))
            throw new InvalidOperationException($"A category with the name '{normalizedName}' already exists.");

        category.Name = normalizedName;
        await categoryRepository.UpdateAsync(category, ct);
        return category;
    }

    public async Task<Category> DeleteCategoryAsync(string id, CancellationToken ct = default)
    {
        var categoryId = NormalizeRequiredName(id, "Category ID");
        var category = await categoryRepository.GetByIdAsync(categoryId, ct)
            ?? throw new InvalidOperationException($"Category with ID '{categoryId}' does not exist.");
        await categoryRepository.DeleteAsync(categoryId, ct);
        return category;
    }

    public async Task<Product> AddProductAsync(string name, decimal price, string categoryId, int stock, CancellationToken ct = default)
    {
        var normalizedName = NormalizeRequiredName(name, "Product");
        if (string.IsNullOrWhiteSpace(categoryId))
        {
            throw new ArgumentException("Product category ID cannot be null or whitespace.", nameof(categoryId));
        }
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot be negative.");
        }
        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stock), "Product stock cannot be negative.");
        }

        var normalizedCategoryId = categoryId.Trim();
        var category = await categoryRepository.GetByIdAsync(normalizedCategoryId, ct) ??
            throw new InvalidOperationException($"Category with ID '{normalizedCategoryId}' does not exist.");
        
        if (await productRepository.NameExistsAsync(normalizedName, ct: ct))
        {
            throw new InvalidOperationException($"A product with the name '{normalizedName}' already exists.");
        }

        var product = new Product
        {
            Name = normalizedName,
            Price = price,
            CategoryId = category.Id,
            Stock = stock
        };
        await productRepository.AddAsync(product, ct);
        return product;
    }

    public async Task<Product> UpdateProductAsync(string id, string? name, decimal? price, string? categoryId, int? stock, CancellationToken ct = default)
    {
        var productId = NormalizeRequiredName(id, "Product ID");
        var product = await productRepository.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");

        var updatedName = name is null ? product.Name : NormalizeRequiredName(name, "Product");
        var updatedPrice = price ?? product.Price;
        var updatedStock = stock ?? product.Stock;
        var updatedCategoryId = categoryId is null ? product.CategoryId : NormalizeRequiredName(categoryId, "Category ID");

        if (updatedPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Product price cannot be negative.");
        if (updatedStock < 0)
            throw new ArgumentOutOfRangeException(nameof(stock), "Product stock cannot be negative.");
        if (await categoryRepository.GetByIdAsync(updatedCategoryId, ct) is null)
            throw new InvalidOperationException($"Category with ID '{updatedCategoryId}' does not exist.");
        if (await productRepository.NameExistsAsync(updatedName, productId, ct))
            throw new InvalidOperationException($"A product with the name '{updatedName}' already exists.");

        product.Name = updatedName;
        product.Price = updatedPrice;
        product.CategoryId = updatedCategoryId;
        product.Stock = updatedStock;
        await productRepository.UpdateAsync(product, ct);
        return product;
    }

    public async Task<Product> DeleteProductAsync(string id, CancellationToken ct = default)
    {
        var productId = NormalizeRequiredName(id, "Product ID");
        var product = await productRepository.GetByIdAsync(productId, ct)
            ?? throw new InvalidOperationException($"Product with ID '{productId}' does not exist.");
        await productRepository.DeleteAsync(productId, ct);
        return product;
    }

    private static string NormalizeRequiredName(string name, string entityName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"{entityName} name cannot be null or whitespace.", nameof(name));
        }

        return name.Trim();
    }
}