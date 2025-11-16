using System.Text.Json;
using System.Text.Json.Serialization;

namespace HRM.Helpers
{
    public class CustomDateTimeConverter: JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.GetString() is string dateString)
            {
                if (DateTime.TryParse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind, out var result))
                {
                    return result;
                }
                throw new JsonException($"Invalid date format: {dateString}");
            }
            throw new JsonException("Expected a string for DateTime.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("O"));
        }
    }
}
