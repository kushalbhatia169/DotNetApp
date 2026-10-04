namespace MyApp.Api
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(string id, CancellationToken ct);
        Task<IEnumerable<Category>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken ct);
    }
}
