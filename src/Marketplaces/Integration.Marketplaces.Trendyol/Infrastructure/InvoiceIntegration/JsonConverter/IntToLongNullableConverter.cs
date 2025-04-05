using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.InvoiceIntegration.JsonConverter;

public class IntToLongNullableConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TryGetInt64(out var longValue))
            return longValue;

        if (reader.TryGetInt32(out var intValue))
            return intValue;

        throw new JsonException("Beklenen bir integer değeri.");
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
