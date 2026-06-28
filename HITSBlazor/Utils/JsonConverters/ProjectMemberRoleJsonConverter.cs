using HITSBlazor.Models.Projects.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class ProjectMemberRoleJsonConverter : JsonConverter<ProjectMemberRole>
    {
        public override ProjectMemberRole Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "Member" => ProjectMemberRole.Member,
                "TeamLeader" => ProjectMemberRole.TeamLeader,
                "Initiator" => ProjectMemberRole.Initiator,
                _ => throw new JsonException($"Unknown project member role type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            ProjectMemberRole value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
