using System.Text.Json.Serialization;

namespace HITSBlazor.Pages.Register
{
    public class RegisterModel
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("telephone")]
        public string Telephone { get; set; } = string.Empty;

        [JsonPropertyName("study_group")]
        public string StudyGroup { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
