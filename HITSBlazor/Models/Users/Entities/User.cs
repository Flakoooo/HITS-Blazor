using HITSBlazor.Models.Users.Enums;
using System.Text.Json.Serialization;

namespace HITSBlazor.Models.Users.Entities
{
    public class User
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("roles")]
        public List<RoleType> Roles { get; set; } = [];

        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = string.Empty;

        [JsonPropertyName("study_group")]
        public string StudyGroup { get; set; } = string.Empty;

        public RoleType? Role { get; set; }
    }
}
