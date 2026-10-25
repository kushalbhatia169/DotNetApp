using System.Text.Json.Serialization;

namespace MyApp.Domain.Entities;

public class Category : BaseEntity
{
    [JsonPropertyName("name")]
    public required string Name { get; set; } = string.Empty;
}