using HITSBlazor.Utils.JsonConverters;
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

        static Settings()
        {
            BaseJsonOptions.Converters.Add(new RoleTypeJsonConverter());
            BaseJsonOptions.Converters.Add(new SkillTypeJsonConverter());
            BaseJsonOptions.Converters.Add(new IdeaStatusTypeJsonConverter());
            BaseJsonOptions.Converters.Add(new TeamRequestStatusJsonConverter());
            BaseJsonOptions.Converters.Add(new MarketStatusJsonConverter());
            BaseJsonOptions.Converters.Add(new ProjectMemberRoleJsonConverter());
            BaseJsonOptions.Converters.Add(new TaskStatusJsonConverter());
            BaseJsonOptions.Converters.Add(new ProjectStatusJsonConverter());
            BaseJsonOptions.Converters.Add(new SprintStatusJsonConverter());
        }
    }
}
