using System.Text.Json.Serialization;

namespace MyApp.Domain.Entities;

public abstract class BaseEntity
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}