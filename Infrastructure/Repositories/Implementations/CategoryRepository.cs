using Microsoft.EntityFrameworkCore;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.Common;

namespace MyApp.Infrastructure.Repositories.Implementations
{
    public class CategoryRepository(AppDbContext dbContext) : GenericEfRepository<Category>(dbContext), ICategoryRepository
    {
        public Task<bool> NameExistsAsync(string name, string? exceptId = null, CancellationToken ct = default)
        {
            return Context.Categories
                .AsNoTracking()
                .AnyAsync(category => category.Id != exceptId && EF.Functions.Collate(category.Name, "NOCASE") == name, ct);
        }
    }
}
