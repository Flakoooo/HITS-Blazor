using System.Text.Json;

namespace HITSBlazor.Utils
{
    public static class Settings
    {
        public static string HttpClientName => "HITSClient";
        public static string DateFormat => "yyyy-MM-ddTHH:mm:ssZ";
        public static JsonSerializerOptions BaseJsonOptions => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        public static JsonSerializerOptions UserJsonOptions { get; }

        static Settings()
        {
            UserJsonOptions = BaseJsonOptions;

            UserJsonOptions.Converters.Add(new RoleTypeJsonConverter());
        }
    }
}
