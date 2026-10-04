using System.Text.Json.Serialization;

namespace MyApp.Api
{
    public class Product : BaseEntity
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("categoryId")]
        public string CategoryId { get; set; } = string.Empty;
        [JsonPropertyName("stock")]
        public int Stock { get; set; }
    }
}
