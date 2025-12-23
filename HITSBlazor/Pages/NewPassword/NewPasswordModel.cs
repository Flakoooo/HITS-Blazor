using System.Text.Json.Serialization;

namespace HITSBlazor.Pages.NewPassword
{
    public class NewPasswordModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}
