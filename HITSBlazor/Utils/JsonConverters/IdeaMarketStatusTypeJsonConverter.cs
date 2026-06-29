using HITSBlazor.Models.Markets.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class IdeaMarketStatusTypeJsonConverter : JsonConverter<IdeaMarketStatusType>
    {
        public override IdeaMarketStatusType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "RecruitmentIsOpen" => IdeaMarketStatusType.RecruitmentIsOpen,
                "RecruitmentIsClosed" => IdeaMarketStatusType.RecruitmentIsClosed,
                "Project" => IdeaMarketStatusType.Project,
                _ => throw new JsonException($"Unknown idea market status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            IdeaMarketStatusType value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
