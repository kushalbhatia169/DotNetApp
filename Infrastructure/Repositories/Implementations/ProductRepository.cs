using Microsoft.EntityFrameworkCore;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;

namespace MyApp.Infrastructure.Repositories.Implementations
{
    public class ProductRepository(AppDbContext dbContext) : GenericEfRepository<Product>(dbContext), IProductRepository
    {
        public Task<bool> NameExistsAsync(string name, string? exceptId = null, CancellationToken ct = default)
        {
            return Context.Products
                .AsNoTracking()
                .AnyAsync(product => product.Id != exceptId && EF.Functions.Collate(product.Name, "NOCASE") == name, ct);
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(string categoryId, CancellationToken ct = default)
        {
            return await Context.Products
                .AsNoTracking()
                .Where(product => EF.Functions.Collate(product.CategoryId, "NOCASE") == categoryId)
                .ToListAsync(ct);
        }
    }
}
