using System.Text.Json.Serialization;

namespace HITSBlazor.Models.Tests.Entities
{
    public class Test
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("test_name")]
        public string TestName { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
