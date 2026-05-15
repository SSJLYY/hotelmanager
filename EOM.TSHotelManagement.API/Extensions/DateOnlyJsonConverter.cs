using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EOM.TSHotelManagement.WebApi
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return default;

            var stringValue = reader.GetString();

            if (string.IsNullOrWhiteSpace(stringValue))
                return default;

            if (DateOnly.TryParse(stringValue, out var result))
                return result;

            return default;
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateOnly value,
            JsonSerializerOptions options)
        {
            if (value == default)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.ToString(DateFormat));
            }
        }
    }
}