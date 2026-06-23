using HITSBlazor.Models.Ideas.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class IdeaStatusTypeJsonConverter : JsonConverter<IdeaStatusType>
    {
        public override IdeaStatusType Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "New" => IdeaStatusType.New,
                "OnEditing" => IdeaStatusType.OnEditing,
                "OnApproval" => IdeaStatusType.OnApproval,
                "OnConfirmation" => IdeaStatusType.OnConfirmation,
                "Confirmed" => IdeaStatusType.Confirmed,
                "OnMarket" => IdeaStatusType.OnMarket,
                _ => throw new JsonException($"Unknown idea status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            IdeaStatusType value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
