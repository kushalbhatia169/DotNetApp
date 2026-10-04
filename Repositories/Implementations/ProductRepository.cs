using MyApp.Api.Infrastructure.Repositories.Common;
using MyApp.Api.Repositories.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Path = System.IO.Path;

namespace MyApp.Api
{
    public class ProductRepository(Microsoft.AspNetCore.Hosting.IWebHostEnvironment env, ICategoryRepository categoryRepository) : GenericJsonRepository<Product>(Path.Combine(env.ContentRootPath, "Data", "products.json")), IProductRepository
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));

        public override async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            var products = await GetAllAsync(cancellationToken);

            var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID '{product.CategoryId}' does not exist.");
            }

            if (products.Any(p => p.Name == product.Name))
            {
                throw new InvalidOperationException($"A product with the name '{product.Name}' already exists.");
            }

            if (string.IsNullOrWhiteSpace(product.Id)) product.Id = Guid.NewGuid().ToString();
            products.Add(product);
            await SaveAllAsync(products, cancellationToken);
        }

        // New specific call for Product
        public async Task<List<Product>> GetProductsByCategoryIdAsync(string categoryId, CancellationToken ct = default)
        {
            var products = await GetAllAsync(ct);
            return products.Where(p => p.CategoryId.Equals(categoryId, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
