using System.Text.Json;

namespace MyApp.Api
{
    public class ProductRepository(string filePath): IProductRepository
    {
        private readonly string _filePath = filePath;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            if (!File.Exists(_filePath)) return [];

            using var stream = File.OpenRead(_filePath);
            var products = await JsonSerializer.DeserializeAsync<List<Product>>(stream, cancellationToken: cancellationToken);
            return products ?? [];
        }

        public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (!File.Exists(_filePath))
                {
                    return [];
                }
                var json = File.OpenRead(_filePath);
                return await JsonSerializer.DeserializeAsync<List<Product>>(json, cancellationToken: cancellationToken) ?? [];
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task SaveProductsAsync(List<Product> products, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(products);
                await File.WriteAllTextAsync(_filePath, json, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task AddProductAsync(Product product, CancellationToken cancellationToken)
        {
            var products = await GetProductsAsync(cancellationToken);
            products.Add(product);
            await SaveProductsAsync(products, cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken)
        {
            var products = await GetProductsAsync(cancellationToken);
            return products.FirstOrDefault(p => p.Id.ToString() == id);
        }

        public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var products = await GetProductsAsync(cancellationToken);
            return products.FirstOrDefault(p => p.Id.ToString() == id.ToString());
        }
    }
}
