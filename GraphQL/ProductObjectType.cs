using HotChocolate.Types;
using MyApp.Api.Repositories.Interfaces;

namespace MyApp.Api.GraphQL
{
    public class ProductObjectType : ObjectType<Product>
    {
        protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
        {
            descriptor.Field(p => p.Id);
            descriptor.Field(p => p.Name);
            descriptor.Field(p => p.Price);
            descriptor.Field(p => p.Stock);
            descriptor.Field(p => p.CategoryId);

            // Custom Resolver for Category - resolve via repository
            descriptor.Field("category")
                .Type<ObjectType<Category>>()
                .Resolve(async context =>
                {
                    var product = context.Parent<Product>();
                    var repo = context.Service<ICategoryRepository>();
                    return await repo.GetByIdAsync(product.CategoryId, context.RequestAborted);
                });
        }
    }
}
