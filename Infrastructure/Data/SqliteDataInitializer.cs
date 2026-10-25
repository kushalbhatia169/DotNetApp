using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MyApp.Domain.Entities;
using Path = System.IO.Path;

namespace MyApp.Infrastructure.Data;

public static class SqliteDataInitializer
{
    public static async Task ImportLegacyJsonAsync(AppDbContext dbContext, string contentRootPath, CancellationToken ct = default)
    {
        if (await dbContext.Products.AnyAsync(ct) || await dbContext.Categories.AnyAsync(ct))
            return;

        var dataDirectory = Path.Combine(contentRootPath, "Data");
        var categoriesPath = Path.Combine(dataDirectory, "categories.json");
        var productsPath = Path.Combine(dataDirectory, "products.json");

        if (!File.Exists(categoriesPath) && !File.Exists(productsPath))
            return;

        if (!File.Exists(categoriesPath) || !File.Exists(productsPath))
            throw new FileNotFoundException("Both legacy category and product JSON files are required for the initial SQLite import.");

        await using var categoriesStream = File.OpenRead(categoriesPath);
        var categories = await JsonSerializer.DeserializeAsync<List<Category>>(categoriesStream, cancellationToken: ct)
            ?? throw new InvalidDataException($"Could not deserialize legacy category data from '{categoriesPath}'.");

        await using var productsStream = File.OpenRead(productsPath);
        var products = await JsonSerializer.DeserializeAsync<List<Product>>(productsStream, cancellationToken: ct)
            ?? throw new InvalidDataException($"Could not deserialize legacy product data from '{productsPath}'.");

        foreach (var entity in categories.Cast<BaseEntity>().Concat(products))
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
                entity.Id = Guid.NewGuid().ToString();
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        await dbContext.Categories.AddRangeAsync(categories, ct);
        await dbContext.Products.AddRangeAsync(products, ct);
        await dbContext.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
