using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyApp.Api
{
    public abstract class BaseEntity
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
    }
}
