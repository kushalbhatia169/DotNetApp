using HotChocolate.Authorization;

namespace MyApp.Api.GraphQL.Queries
{
    // High-performance Hot Chocolate Extensions
    public class ProductQuery
    {
        [Authorize]
        public async Task<IEnumerable<Product>> GetProductsAsync(
            IProductRepository repository,
            CancellationToken ct)
        {
            return await repository.GetAllProductsAsync(ct);
        }

        // Role-based Auth: Token ke andar Role Claim = 'Admin' hona chahiye
        [Authorize/*(Roles = new[] { "Admin" })*/]
        public async Task<Product?> GetProductByIdAsync(
            string id,
            IProductRepository repository,
            CancellationToken ct)
        {
            return await repository.GetProductByIdAsync(id, ct);
        }
    }
}
