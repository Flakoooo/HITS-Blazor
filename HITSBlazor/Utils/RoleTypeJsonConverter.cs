using HITSBlazor.Models.Users.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils
{
    public class RoleTypeJsonConverter : JsonConverter<RoleType>
    {
        public override RoleType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "Initiator" => RoleType.Initiator,
                "TeamOwner" => RoleType.TeamOwner,
                "TeamLeader" => RoleType.TeamLeader,
                "Member" => RoleType.Member,
                "ProjectOffice" => RoleType.ProjectOffice,
                "Expert" => RoleType.Expert,
                "Admin" => RoleType.Admin,
                "Teacher" => RoleType.Teacher,
                _ => throw new JsonException($"Unknown role type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            RoleType value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
