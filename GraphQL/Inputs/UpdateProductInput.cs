namespace MyApp.Api.GraphQL.Inputs;

public sealed class UpdateProductInput
{
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public string? CategoryId { get; set; }
    public int? Stock { get; set; }
}
