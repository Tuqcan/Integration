using Integration.Hub;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;

// GET .../products/approved (V2) response. Nested: content -> variants[].
// Sadece flatten'da kullanilan alanlar modellenmistir; System.Text.Json eslesmeyen
// JSON alanlarini yok sayar.
public class ApprovedProductsV2Response : PaginationModel, IResponseModel
{
    public string? NextPageToken { get; set; }
    public List<ApprovedProductV2> Content { get; set; } = [];
}

public class ApprovedProductV2 : IResponseModel
{
    public long ContentId { get; set; }
    public string ProductMainId { get; set; }

    public ProductBrandV2 Brand { get; set; }
    public ProductCategoryV2 Category { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public List<FilterProductImageResponseModel> Images { get; set; } = [];
    public List<FilterProductAttributeResponseModel> Attributes { get; set; } = [];
    public List<ApprovedVariantV2> Variants { get; set; } = [];
}

public class ProductBrandV2 : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ProductCategoryV2 : IResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ApprovedVariantV2 : IResponseModel
{
    public long VariantId { get; set; }
    public int SupplierId { get; set; }
    public string Barcode { get; set; }
    public decimal? Commission { get; set; }

    public string? ProductUrl { get; set; }
    public string? StockCode { get; set; }

    public bool OnSale { get; set; }
    public bool Locked { get; set; }
    public bool Archived { get; set; }
    public bool Blacklisted { get; set; }

    public int? VatRate { get; set; }   // Trendyol null donebiliyor -> nullable

    /// <summary>
    /// Urun mensei (ISO 3166-1 alpha-2, canli ornek: "CN").
    ///
    /// Trendyol menseiyi attribute'tan bagimsiz STANDART bir alana tasiyor ve
    /// 23.10.2026'dan itibaren urun YAZMA isteklerinde ZORUNLU olacak. Biz su an
    /// yazmadigimiz icin sert bir kirilma yok; okuma tarafinda alan zaten dolu geliyor
    /// (kanit: T1_approved_hypercep.json variant.origin, T3_unapproved_hypercep.json item.origin).
    ///
    /// CIFTE KAYNAK UYARISI: ayni bilgi content.attributes icinde attribute 1192
    /// ("Mensei", deger "CN") olarak DA geliyor. QnA promptu menseiyi O attribute'tan
    /// aliyor; bu alan promptta AYRICA gosterilMEZ (aksi halde "Mensei: CN" iki kez basar).
    /// Bu alanin amaci raporlama ve ileriye donuk yazma destegidir.
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Satis kanallari. Canli ornek: <c>["CORE"]</c> (Luxe satisi yok).
    ///
    /// Modellenmesi ucuz; Luxe satan bir kullanici geldiginde hazir olsun diye
    /// okunuyor. Su an DB'ye YAZILMIYOR - yazilmasi icin once "hangi karari
    /// degistiriyor?" sorusunun cevabi olmali.
    /// </summary>
    public List<string> Channels { get; set; } = [];

    // V1 createDateTime/lastUpdateDate karsiligi (birebir ayni epoch ms).
    public long SellerCreatedDate { get; set; }
    public long SellerModifiedDate { get; set; }

    /// <summary>
    /// VARYANTA OZGU ozellikler (Beden gibi 'varianter=true' olanlar).
    ///
    /// Olculen magazalarda (kilif/koruyucu, kategori 766/5511) 653/653 varyantta BOS
    /// geliyor - o kategorilerde varianter=true attribute YOK, Renk 'slicer' (ayri kart).
    /// Ama giyim kategorilerinde Beden varianter=true (canli kanit: CAT_V2_384.json,
    /// attribute 338) ve varyant ozellikleri BU DIZIDE gelir.
    ///
    /// Modellenmezse giyim satan ILK kullanicida tum varyantlar AYNI gorunur ve
    /// QnA'nin <variants> blogu uretilemez - sessiz, gec fark edilen bir kayip.
    /// </summary>
    public List<FilterProductAttributeResponseModel> Attributes { get; set; } = [];

    public ProductDeliveryOptionsV2? DeliveryOptions { get; set; }
    public ProductStockV2? Stock { get; set; }
    public ProductPriceV2? Price { get; set; }
}

public class ProductDeliveryOptionsV2 : IResponseModel
{
    public int? DeliveryDuration { get; set; }
}

public class ProductStockV2 : IResponseModel
{
    public int? Quantity { get; set; }
}

public class ProductPriceV2 : IResponseModel
{
    public decimal? SalePrice { get; set; }
    public decimal? ListPrice { get; set; }
}
