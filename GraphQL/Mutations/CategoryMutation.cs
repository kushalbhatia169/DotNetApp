using HotChocolate.Authorization;
using MyApp.Api.GraphQL.Inputs;
using MyApp.Application.Abstractions;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;

namespace MyApp.Api.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public sealed class CategoryMutation
{
    [Authorize(Roles = new[] { "Admin" })]
    public Task<Category> AddCategoryAsync(
        AddCategoryInput input,
        [Service] ICatalogService catalogService,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        return catalogService.AddCategoryAsync(input.Name, ct);
    }

    [Authorize(Roles = new[] { "Admin" })]
    public Task<Category> UpdateCategoryAsync(
        string categoryId,
        UpdateCategoryInput input,
        [Service] ICatalogService catalogService,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(input);
        return catalogService.UpdateCategoryAsync(categoryId, input.Name, ct);
    }

    [Authorize(Roles = new[] { "Admin" })]
    public async Task<List<Category>> UpdateBulkCategoryAsync(
        List<string> categoryIds,
        UpdateCategoryInput input,
        [Service] ICategoryRepository categoryRepository,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            throw new NullReferenceException("Category name cannot be null or whitespace.");
        }

        var categories = new List<Category>();
        foreach (var categoryId in categoryIds)
        {
            var category = await categoryRepository.GetByIdAsync(categoryId, ct)
                ?? throw new InvalidOperationException($"Category with ID '{categoryId}' was not found.");
            categories.Add(category);
        }

        foreach (var category in categories)
        {
            category.Name = input.Name;
            await categoryRepository.UpdateAsync(category, ct);
        }

        return categories;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public Task<Category> DeleteCategoryAsync(
        string categoryId,
        [Service] ICatalogService catalogService,
        CancellationToken ct) =>
        catalogService.DeleteCategoryAsync(categoryId, ct);

    [Authorize(Roles = new[] { "Admin" })]
    public async Task<List<Category>> DeleteBulkCategoryAsync(
        List<string> categoryIds,
        [Service] ICategoryRepository categoryRepository,
        CancellationToken ct)
    {
        var categories = new List<Category>();
        foreach (var categoryId in categoryIds)
        {
            var category = await categoryRepository.GetByIdAsync(categoryId, ct)
                ?? throw new InvalidOperationException($"Category with ID '{categoryId}' was not found.");
            categories.Add(category);
        }
        foreach (var category in categories)
        {
            await categoryRepository.DeleteAsync(category.Id, ct);
        }
        return categories;
    }
}
