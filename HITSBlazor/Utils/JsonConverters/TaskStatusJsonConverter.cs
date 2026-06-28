using System.Text.Json;
using System.Text.Json.Serialization;

using HITSTaskStatus = HITSBlazor.Models.Projects.Enums.TaskStatus;

namespace HITSBlazor.Utils.JsonConverters
{
    public class TaskStatusJsonConverter : JsonConverter<HITSTaskStatus>
    {
        public override HITSTaskStatus Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            var stringValue = reader.GetString();

            return stringValue switch
            {
                "InBackLog" => HITSTaskStatus.InBackLog,
                "NewTask" => HITSTaskStatus.NewTask,
                "InProgress" => HITSTaskStatus.InProgress,
                "OnVerification" => HITSTaskStatus.OnVerification,
                "OnModification" => HITSTaskStatus.OnModification,
                "Done" => HITSTaskStatus.Done,
                _ => throw new JsonException($"Unknown task status type: {stringValue}")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            HITSTaskStatus value,
            JsonSerializerOptions options
        ) => writer.WriteStringValue(value.ToString());
    }
}
