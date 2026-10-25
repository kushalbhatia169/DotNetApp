using HotChocolate.Authorization;
using HotChocolate.Types;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Api.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public sealed class CategoryQuery
{
    [Authorize]
    public Task<List<Category>> GetCategoriesAsync(ICategoryRepository repository, CancellationToken ct) =>
        repository.GetAllAsync(ct);

    [Authorize]
    public Task<Category?> GetCategoryByIdAsync(string id, ICategoryRepository repository, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Category ID cannot be null or whitespace.", nameof(id));

        return repository.GetByIdAsync(id, ct);
    }

    [Authorize]
    public Task<List<Category>> GetCategoriesByIdsAsync(List<string> ids, ICategoryRepository repository, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(ids);
        return repository.GetByKeysAsync(ids, ct);
    }
}
