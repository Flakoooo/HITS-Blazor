using HITSBlazor.Models.Markets.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HITSBlazor.Utils.JsonConverters
{
    public class MarketStatusJsonConverter : JsonConverter<MarketStatus>
    {
        public override MarketStatus Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "New" => MarketStatus.New,
                "Active" => MarketStatus.Active,
                "Done" => MarketStatus.Done,
                _ => throw new JsonException($"Unknown market status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            MarketStatus value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
