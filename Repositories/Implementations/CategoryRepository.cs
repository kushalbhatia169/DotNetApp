using System.Text.Json;

namespace MyApp.Api
{
    public class CategoryRepository(string filePath) : ICategoryRepository
    {
        private readonly string _filePath = filePath;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var categories = await GetCategoriesAsync(cancellationToken);
            return categories.FirstOrDefault(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetByKeysAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var categories = await GetCategoriesAsync(cancellationToken);
            var keySet = new HashSet<string>(keys);
            return categories.Where(c => keySet.Contains(c.Id)).ToList();
        }

        private async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (!File.Exists(_filePath)) return new List<Category>();
                using var stream = File.OpenRead(_filePath);
                return await JsonSerializer.DeserializeAsync<List<Category>>(stream, cancellationToken: cancellationToken) ?? new List<Category>();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
