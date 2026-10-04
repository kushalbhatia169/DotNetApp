namespace MyApp.Api
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken ct);
        Task<Product?> GetProductByIdAsync(string id, CancellationToken ct);
        Task<Product?> GetProductByIdAsync(Guid id, CancellationToken ct);
        Task<List<Product>> GetProductsAsync(CancellationToken ct);
        Task SaveProductsAsync(List<Product> products, CancellationToken ct);
        Task AddProductAsync(Product product, CancellationToken ct);
    }
}
