using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;

/// <summary>
/// V2 kategori-ozellik DEGER ucunun yaniti:
/// <c>GET product/categories/{categoryId}/attributes/{attributeId}/values?page=0&amp;size=1000</c>
///
/// V1'de degerler kategori-ozellik yanitinin icinde (<c>categoryAttributes[].attributeValues</c>)
/// gomulu geliyordu. V2 bu alani KALDIRDI (canli kanit: ayni kategori 766 icin V1 97.620 byte /
/// 18-18 attributeValues, V2 3.296 byte / 0-18). Degerler artik bu ayri, sayfali uctan cekilir.
///
/// Kanit dosyasi: developer-md/trendyol-v2-fixtures/VALS_766_348.json (26 deger, tek sayfa).
/// Sayfalama canli dogrulandi: attribute 292 -> 2.139 deger / 3 sayfa.
/// </summary>
public class GetCategoryAttributeValuesResponseModel : PaginationModel, IResponseModel
{
    public List<CategoryAttributeValueItemModel> Content { get; set; } = [];
}

/// <summary>
/// Tek bir ozellik degeri. Alan adlari V1'deki <c>id</c>/<c>name</c> DEGIL,
/// <c>attributeValueId</c>/<c>attributeValue</c> (canli yanittan birebir).
/// </summary>
public class CategoryAttributeValueItemModel : IResponseModel
{
    public int AttributeValueId { get; set; }
    public string AttributeValue { get; set; } = string.Empty;
}
