using HITSBlazor.Models.Common.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class SkillTypeJsonConverter : JsonConverter<SkillType>
    {
        public override SkillType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "Language" => SkillType.Language,
                "Framework" => SkillType.Framework,
                "Database" => SkillType.Database,
                "Devops" => SkillType.Devops,
                _ => throw new JsonException($"Unknown skill type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            SkillType value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
