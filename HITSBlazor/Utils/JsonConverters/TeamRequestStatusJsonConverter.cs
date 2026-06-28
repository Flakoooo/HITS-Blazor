using HITSBlazor.Models.Ideas.Enums;
using HITSBlazor.Models.Teams.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class TeamRequestStatusJsonConverter : JsonConverter<TeamRequestStatus>
    {
        public override TeamRequestStatus Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "New" => TeamRequestStatus.New,
                "Annulled" => TeamRequestStatus.Annulled,
                "Accepted" => TeamRequestStatus.Accepted,
                "Canceled" => TeamRequestStatus.Canceled,
                "Withdrawn" => TeamRequestStatus.Withdrawn,
                _ => throw new JsonException($"Unknown team request/invitation status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            TeamRequestStatus value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
