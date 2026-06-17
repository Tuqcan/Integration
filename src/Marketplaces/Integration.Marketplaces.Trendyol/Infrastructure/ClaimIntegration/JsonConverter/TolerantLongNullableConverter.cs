using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ClaimIntegration.JsonConverter;

/// <summary>
/// long? deserialize ederken degeri Int64'e sigmiyorsa (ornek: 20 haneli kargo takip no)
/// exception firlatmak yerine null doner. Boylece tek bir bozuk alan tum claims sayfasini
/// deserialize hatasina dusurmez; claim yine senkronlanir, sadece o alan null olur.
/// </summary>
public class TolerantLongNullableConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var longValue))
            return longValue;

        // Int64'e sigmayan sayi veya string olarak gelen deger: hata firlatma, null don.
        reader.Skip();
        return null;
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteNumberValue(value.Value);
        else
            writer.WriteNullValue();
    }
}
