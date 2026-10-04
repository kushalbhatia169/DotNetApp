using System.Text.Json.Serialization;

namespace MyApp.Api
{
    public class Category
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; } = string.Empty;
        [JsonPropertyName("name")]
        public required string Name { get; set; } = string.Empty;
    }
}
