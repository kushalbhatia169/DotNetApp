namespace MyApp.Api.GraphQL.Inputs
{
    public record AddProductInput(
        string Name,
        decimal Price,
        string CategoryId,
        int Stock
    );
}
