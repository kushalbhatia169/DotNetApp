using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<bool> NameExistsAsync(string name, string? exceptId = null, CancellationToken ct = default);
}