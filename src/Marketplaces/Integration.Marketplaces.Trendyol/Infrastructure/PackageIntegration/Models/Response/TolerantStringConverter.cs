using System.Buffers;   // ReadOnlySequence<byte>.ToArray() bu ad alanindan gelir
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Response;

/// <summary>
/// Sayi/bool olarak gelebilen bir alani STRING olarak okur.
///
/// ############ NEDEN VAR - 01.09.2026 URETIM OLAYI ############
/// Faz 6'da eklenen <c>cancelReasonCode</c> alani modele <c>string?</c> olarak
/// yazildi. Trendyol onu SAYI gonderiyor:
///
///     System.Text.Json.JsonException: The JSON value could not be converted to
///     System.String. Path: $.content[3].lines[0].cancelReasonCode
///
/// Etkisi TEK ALANLA SINIRLI KALMADI: System.Text.Json bir alanda patlayinca TUM
/// GOVDEYI birakiyor, yani 200 paketlik sayfanin TAMAMI kayboluyor. Deploy sonrasi
/// HyperCep (193500) icin 4 turun 4'u de dustu ve hic siparis yazilmadi.
///
/// KOK NEDEN - TIP VARSAYIMI DOGRULANMADI: alanin DOLULUK orani olculmustu
/// (%16,7 - yalniz iptal satirlarinda) ama JSON TIPI hic gorulmemisti; repodaki
/// ornek pakette iptal edilmis satir olmadigi icin fixture'da bu alan YOK.
/// "Ornekte gormedim" ile "tipi sudur" ayri seylerdir - ayni ders plan bolum 1.9'da
/// EtgbNo icin de yazilmisti.
///
/// ############ NEDEN "int?" YAPMIYORUZ ############
/// Bugun sayi geliyor, ama Trendyol'un ayni alani baska bir kayitta metin gondermesi
/// (ornegin "CUSTOMER_REQUEST") bizi ayni yere geri dusururdu; dokumanda tip YAZILI
/// DEGIL. Bu donusturucu HER IKI SEKLI de kabul eder ve DB tarafi degismez
/// (nvarchar(50) kolonu oldugu gibi kalir, migration GEREKMEZ).
/// #################################################
/// </summary>
public sealed class TolerantStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.String:
                return reader.GetString();

            // Sayi: ham metnini oldugu gibi al. GetInt64/GetDecimal DENENMEZ cunku
            // alan bir KOD'dur, sayi degil - basindaki sifirlar ("007") ya da
            // long'a sigmayan bir deger sessizce bozulmamali.
            case JsonTokenType.Number:
                return System.Text.Encoding.UTF8.GetString(
                    reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan);

            case JsonTokenType.True:
                return "true";
            case JsonTokenType.False:
                return "false";

            default:
                // Beklenmeyen bir sekil (nesne/dizi): TUM GOVDEYI dusurmek yerine bu
                // alani atla ve null don. Bir kod alani ugruna 200 paket kaybedilmez.
                reader.Skip();
                return null;
        }
    }

    /// <summary>
    /// Yalniz OKUMA tarafi kullaniliyor (bu modeller yanit modelidir). Yazma, string
    /// icin varsayilan davranisi korur ki modelin bir yere serialize edilmesi
    /// gerekirse sekil degismesin.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null) writer.WriteNullValue();
        else writer.WriteStringValue(value);
    }
}
