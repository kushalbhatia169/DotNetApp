using Microsoft.EntityFrameworkCore;
using MyApp.Application.Abstractions.Repositories;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.Common;

public abstract class GenericEfRepository<T>(AppDbContext dbContext) : IGenericRepository<T> where T : BaseEntity
{
    protected AppDbContext Context { get; } = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private DbSet<T> Entities => Context.Set<T>();

    public async Task<List<T>> GetAllAsync(CancellationToken ct = default)
    {
        return await Entities.AsNoTracking().ToListAsync(ct);
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(entity => entity.Id == id, ct);
    }

    public virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(entity.Id))
        {
            entity.Id = Guid.NewGuid().ToString();
        }

        await Entities.AddAsync(entity, ct);
        await Context.SaveChangesAsync(ct);
    }
    public async Task<bool> ExistsAsync(string id, CancellationToken ct = default)
    {
        return await Entities.AsNoTracking().AnyAsync(entity => entity.Id == id, ct);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (!await ExistsAsync(entity.Id, ct))
        {
            throw new InvalidOperationException($"Entity of type '{typeof(T).Name}' with ID '{entity.Id}' not found.");
        }

        Entities.Update(entity);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (!await ExistsAsync(id, ct))
        {
            throw new InvalidOperationException($"Entity of type '{typeof(T).Name}' with ID '{id}' not found.");
        }

        var entity = await Entities.FindAsync([id], ct)??
            throw new InvalidOperationException($"Entity of type '{typeof(T).Name}' with ID '{id}' not found.");
        Entities.Remove(entity);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task<List<T>> GetByKeysAsync(List<string> keys, CancellationToken ct = default)
    {
        var keySet = keys.ToArray();
        return await Entities
            .AsNoTracking()
            .Where(entity => keySet.Contains(entity.Id))
            .ToListAsync(ct);
    }

    public virtual async Task UpdateAll(List<T> entities, CancellationToken ct = default)
    {
        foreach (var entity in entities)
        {
            if (string.IsNullOrWhiteSpace(entity.Id) || ExistsAsync(entity.Id, ct).Result == false)
            {
                entity.Id = Guid.NewGuid().ToString();
            }
        }
        Entities.UpdateRange(entities);
        await Context.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAll(List<string> ids, CancellationToken ct = default)
    {
        var entitiesToDelete = await Entities.Where(entity => ids.Contains(entity.Id)).ToListAsync(ct);
        if (entitiesToDelete.Count != ids.Count)
        {
            throw new InvalidOperationException("One or more entities to delete were not found.");
        }

        //filter entnties to delete to only those that exist in the database
        var existingEntitiesToDelete = await ExistsAsync(entitiesToDelete[0].Id, ct);

        Entities.RemoveRange(entitiesToDelete);
        await Context.SaveChangesAsync(ct);
    }
}
