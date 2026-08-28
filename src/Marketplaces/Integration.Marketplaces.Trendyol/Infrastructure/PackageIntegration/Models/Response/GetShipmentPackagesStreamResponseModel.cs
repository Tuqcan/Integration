using Integration.Hub;
using System.Text.Json.Serialization;

namespace Integration.Marketplaces.Trendyol.Infrastructure.PackageIntegration.Models.Response;

/// <summary>
/// orders/stream zarfi.
///
/// ############ GOVDE v2/orders ILE BIREBIR AYNI ############
/// 28.08.2026 canli dogrulamasi:
///   * ayni shipmentPackageId icin JSON karsilastirmasi -> true
///   * ayni 5 gunluk pencere: v2 273 kayit, stream 273 kayit, KUME FARKI 0
///   * cursor sayfalari arasinda tekrar 0, lastModifiedDate DESC sira bozulmasi 0
///
/// Bu yuzden AYNI satir modelini yeniden kullanir - IKINCI BIR PAKET MODELI ACILMAZ.
/// Ikinci model acilsaydi Faz 1'in [JsonPropertyName] kilitleri IKI YERDE tutulmak
/// zorunda kalir ve biri unutuldugunda SESSIZCE saparodi.
/// #########################################################
///
/// ⛔ PaginationModel'DEN TUREMEZ. Zarfta totalElements / totalPages / page YOK;
/// turetmek onlari 0 olarak gosterip "0 kayit var" yanilgisina yol acardi.
/// Akisi <see cref="HasMore"/> yurutur, sayfa sayisi DEGIL.
/// </summary>
public class GetShipmentPackagesStreamResponseModel : IResponseModel
{
    /// <summary>
    /// Akisin TEK dogal bitis kosulu. false olunca durulur.
    /// </summary>
    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    /// <summary>
    /// OPAK. Ayristirilmaz, degistirilmez, KALICI olarak saklanmaz.
    ///
    /// Icerigi base64 { "s":[sellerId, ms, "pkgId"], "f":{...} } olsa da bu bir
    /// UYGULAMA DETAYIDIR; bugun cozebiliyor olmamiz yarin da cozebilecegimiz
    /// anlamina gelmez. Bozuk cursor HTTP 400 doner (olculdu).
    ///
    /// hasMore=false oldugunda BOS STRING doner (olculdu).
    ///
    /// KALICI SAKLANMAZ: saklanan bir cursor, filtre penceresi kaydigi anda
    /// (bir sonraki tur farkli start/end uretir) HTTP 400 verirdi - cursor + farkli
    /// tarih penceresi kombinasyonu olculdu ve 400 dondu. Cursor'in omru AKISIN OMRUDUR.
    /// </summary>
    [JsonPropertyName("nextCursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// DIKKAT: bu alan DONEN KAYIT ADEDIDIR, istenen sayfa boyu DEGIL.
    /// Bos pencerede 0 doner. Sayfalama karari icin KULLANMAYIN - akisi hasMore yurutur.
    /// (v2/orders'taki "size" alaninin ayni tuzagi - bkz. Faz 4.2.)
    /// </summary>
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("content")]
    public List<GetShipmentPackagePackageResponseModel> Content { get; set; } = new();
}
