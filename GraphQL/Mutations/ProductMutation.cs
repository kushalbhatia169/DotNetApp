using HotChocolate.Authorization;
using MyApp.Api.GraphQL.Inputs;
using MyApp.Api.Repositories.Interfaces;

namespace MyApp.Api.GraphQL.Mutations
{
    public class ProductMutation
    {
        [Authorize(Roles = new[] { "Admin" })]
        public async Task<Product> AddProductAsync(
            AddProductInput input,
            [Service] IProductRepository repository,
            CancellationToken ct)
        {
            var newProduct = new Product
            {
                Name = input.Name,
                Price = input.Price,
                CategoryId = input.CategoryId,
                Stock = input.Stock
            };

            await repository.AddAsync(newProduct, ct);
            return newProduct;
        }
    }
}
