using HITSBlazor.Models.Projects.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class ProjectStatusJsonConverter : JsonConverter<ProjectStatus>
    {
        public override ProjectStatus Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "New" => ProjectStatus.Active,
                "Annulled" => ProjectStatus.Done,
                "Accepted" => ProjectStatus.Paused,
                "Canceled" => ProjectStatus.Deleted,
                _ => throw new JsonException($"Unknown project status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            ProjectStatus value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
