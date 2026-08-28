using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;

public class GetCategoryAttributesResponseModel : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public List<CategoryAttributeResponseModel> CategoryAttributes { get; set; }
}

public class CategoryAttributeResponseModel : IResponseModel
{
    public int CategoryId { get; set; }
    public CategoryAttributeInfoModel Attribute { get; set; }
    public bool Required { get; set; }
    public bool AllowCustom { get; set; }
    public bool Varianter { get; set; }
    public bool Slicer { get; set; }
    public bool AllowMultipleAttributeValues { get; set; }

    /// <summary>
    /// V2 BU ALANI DOLDURMAZ - daima <c>null</c> gelir.
    ///
    /// Canli kanit (27.08.2026, kategori 766): V1 ucu 18/18 attribute icin degerleri gomulu
    /// veriyordu (97.620 byte); V2 ucu ayni 18 attribute'u 0 deger ile donuyor (3.296 byte).
    /// Kanit: developer-md/trendyol-v2-fixtures/CAT_V1_766.json vs CAT_V2_766.json
    ///
    /// Degerler artik ayri ve sayfali uctan cekilir:
    /// <see cref="GetCategoryAttributeValuesResponseModel"/> +
    /// ITrendyolProductIntegration.GetCategoryAttributeValuesAsync.
    ///
    /// Alan SILINMEDI ki eski V1 yanitlarini (fixture/regresyon) deserialize eden kod kirilmasin.
    /// Yeni kod BU ALANA GUVENMEMELI: null kontrolu sessizce gecer ve deger senkronu
    /// hic calismadan "basarili" gorunur - Faz 1'in tum gerekcesi budur.
    /// </summary>
    [Obsolete("V2 bu alani doldurmaz (daima null). Degerler icin GetCategoryAttributeValuesAsync kullanin.")]
    public List<CategoryAttributeValueResponseModel> AttributeValues { get; set; }
}

public class CategoryAttributeInfoModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class CategoryAttributeValueResponseModel : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}
