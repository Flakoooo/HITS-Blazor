using HITSBlazor.Models.Projects.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class SprintStatusJsonConverter : JsonConverter<SprintStatus>
    {
        public override SprintStatus Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "Active" => SprintStatus.Active,
                "Done" => SprintStatus.Done,
                _ => throw new JsonException($"Unknown sprint status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            SprintStatus value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
