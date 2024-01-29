using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is  JsonTokenType.String)
        {
            string? timeString = reader.GetString();

            ArgumentNullException.ThrowIfNull(timeString, nameof(timeString));
            return TimeOnly.ParseExact(timeString, "HH:mm", null);

        }
        else
           throw new InvalidCastException("can not cast ");
    
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("HH:mm"));
    }
}