using Integration.Hub;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Request;
using Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Models.Response;
using Integration.Marketplaces.Trendyol.Models.Provider;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration;

public interface ITrendyolProductIntegration
{
    [Obsolete("Path V2'ye guncellendi (sellers/{id}/addresses) ama canli dogrulanmadi. " +
              "Bu uc siparis servis grubuna ait; siparis modulu gecisinde ele alinacak.")]
    public Task<GetSuppliersAddressesResponseModel?> GetSuppliersAddressesAsync();

    /// <summary>
    /// createProduct V2 servisine yapılacak isteklerde gönderilecek kargo 
    /// firma bilgileri ve bu bilgilere ait ID değerleri aşağıdaki tablo üzerindedir. 
    /// Bu değerleri kullanılarak gerekli bilgileri sistemlerinize entegre 
    /// etmeniz gerekmektedir. <br/>
    /// Ürün gönderimi yaparken gönderdiğiniz kargo 
    /// şirketleri, Trendyol sözleşmenizde onayladığınız kargo
    /// firmasından farklı olmamalıdır. Bu durum ürünlerinizi 
    /// yayına çıkmasını engelleyecektir.
    /// </summary>
    /// <returns><see cref="GetProviderResponseModel"/></returns>
    public List<GetProviderResponseModel> GetProviders();

    /// <summary>
    /// createProduct V2 servisine yapılacak isteklerde gönderilecek brandId 
    /// bilgisi bu servis kullanılarak alınacaktır.<br/>
    /// Bir sayfada minumum 1000 adet brand bilgisi alınabilmektedir.<br/>
    /// Marka araması yaparken servise page parametresini kullanarak query 
    /// oluşturmanız gerekmektedir.
    /// </summary>
    /// <returns><see cref="GetBrandsResponseModel"/></returns>
    public Task<GetBrandsResponseModel?> GetBrandsAsync();

    /// <summary>
    /// createProduct V2 servisine yapılacak isteklerde gönderilecek categoryId 
    /// bilgisi bu servis kullanılarak alınacaktır.<br/>
    /// createProduct yapmak için en alt seviyedeki kategori ID 
    /// bilgisi kullanılmalıdır. Seçtiğiniz kategorinin alt kategorileri 
    /// var ise bu kategori bilgisi ile ürün aktarımı yapamazsınız.<br/>
    /// Yeni kategoriler eklenebileceği sebebiyle güncel kategori listesini 
    /// haftalık olarak almanızı öneririz.
    /// </summary>
    /// <returns><see cref="GetCategoryTreeResponseModel"/></returns>
    public Task<GetCategoryTreeResponseModel?> GetCategoryTreeAsync();

    /// <summary>
    /// createProduct V2 servisine yapılacak isteklerde gönderilecek attributes 
    /// bilgileri ve bu bilgilere ait detaylar bu servis kullanılarak alınacaktır.<br/>
    ///createProduct yapmak için en alt seviyedeki kategori ID bilgisi kullanılmalıdır. 
    ///Seçtiğiniz kategorinin alt kategorileri var ise bu kategori bilgisi ile ürün 
    ///aktarımı yapamazsınız.<br/>
    /// Yeni kategorileri özellikleri eklenebileceği sebebiyle güncel kategori 
    /// özellik listesini haftalık olarak almanızı öneririz.
    /// </summary>
    /// <param name="categoryId">Category id</param>
    /// <returns><see cref="GetCategoryAttributesResponseModel"/></returns>
    public Task<GetCategoryAttributesResponseModel?> GetCategoryAttributes(int categoryId);

    /// <summary>
    /// Bir kategori-ozellik ciftinin SECILEBILIR DEGERLERINI sayfali olarak getirir.<br/>
    /// <c>GET product/categories/{categoryId}/attributes/{attributeId}/values</c><br/><br/>
    ///
    /// V1'de degerler <see cref="GetCategoryAttributes"/> yanitinin icinde
    /// (<c>categoryAttributes[].attributeValues</c>) gomulu geliyordu. V2 o alani KALDIRDI
    /// (canli kanit: kategori 766 -> V1 18/18 attribute'ta deger var, V2 0/18).
    /// Deger senkronu artik ZORUNLU olarak bu uctan yapilir.<br/><br/>
    ///
    /// <b>Deger kumesi kategoriden BAGIMSIZDIR</b> (canli dogrulandi: attribute 348 icin
    /// kategori 766 ve 5511 birebir ayni 26 degeri donuyor). Bu yuzden cagiran taraf
    /// kategori x attribute fan-out'u yapmak zorunda DEGIL; attribute basina ilk gorulen
    /// kategori ile tek kez cekmek yeterlidir.<br/><br/>
    ///
    /// Sayfalama: yanit <see cref="Integration.Hub.PaginationModel"/> alanlarini tasir
    /// (<c>totalElements/totalPages/page/size</c>).<br/><br/>
    ///
    /// <b>Sayfa boyutu 1000'de SINIRLI DEGIL</b> (28.08.2026 canli olcumu, attribute 292 =
    /// 2.140 deger): <c>size=1000</c> -> 3 sayfa, <c>size=2000</c> -> 2 sayfa,
    /// <c>size=3000</c> -> <b>1 sayfa</b>. Varsayilan
    /// <see cref="TrendyolProductIntegration.DefaultValuePageSize"/>.<br/><br/>
    ///
    /// <b>⚠️ ARALIK DISI SAYFA ISTEMEYIN.</b> Canli olcum: <c>page=9999</c> -> HTTP 500,
    /// <c>page=-1</c> -> HTTP 500. Dongu <c>totalElements</c> ve "sayfa dolu mu" olcutuyle
    /// durmali, yalnizca <c>totalPages</c>'e guvenmemeli.<br/><br/>
    ///
    /// <b>⚠️ GECERSIZ ID SESSIZDIR.</b> Var olmayan attributeId veya categoryId
    /// <b>HTTP 200 + bos icerik</b> doner (404 DEGIL). Yani "deger yok" ile "boyle bir
    /// ozellik yok" bu uctan AYIRT EDILEMEZ.
    /// </summary>
    /// <param name="categoryId">Ozelligin gorulduğu HERHANGI bir kategori</param>
    /// <param name="attributeId">Degerleri istenen ozellik</param>
    /// <param name="page">0 tabanli sayfa numarasi</param>
    /// <param name="size">Sayfa boyutu (1000 uzeri degerler de onurlandiriliyor)</param>
    /// <param name="ct">Iptal jetonu</param>
    /// <returns><see cref="GetCategoryAttributeValuesResponseModel"/></returns>
    public Task<GetCategoryAttributeValuesResponseModel?> GetCategoryAttributeValuesAsync(
        int categoryId, int attributeId, int page = 0,
        int size = TrendyolProductIntegration.DefaultValuePageSize, CancellationToken ct = default);

    /// <summary>
    /// createProduct V2 servisine yapılacak isteklerde gönderilecek sipariş ve 
    /// sevkiyat kargo firma bilgileri ve bu bilgilere ait ID değerleri bu servis 
    /// kullanılarak alınacaktır.
    /// </summary>
    /// <param name="products"></param>
    /// <returns><see cref="bool"/></returns>
    public Task<bool> CreateProductsV2Async(BulkModel<CreateProductRequestModel> products);

    /// <summary>
    /// Bu method ile Trendyol mağazanızda createProduct V2 servisiyle oluşturduğunuz 
    /// ürünleri güncelleyebilirsiniz.<br/>
    /// Bu servis üzerinden sadece ürün bilgileri güncellenmektedir. Stok ve fiyat 
    /// değerlerini güncellemek için updatePriceAndInventory servisini kullanmanız gerekmektedir.<br/>
    /// Yeni kategori ve kategori özellik değerleri eklenebileceği sebebiyle ürün güncellemelerinizden
    /// önce kullandığınız kategori ve kategori özellik değerlerinin güncel olup olmadığını 
    /// getCategoryTree ve getCategoryAttributes servislerinden kontrol etmenizi öneririz.<br/>
    /// Güncelleme isteğinden sonra "İçerik kontrol bekleniyor." statüsüne geçen ürünleriniz 
    /// satışa açık durumda olabilir. Sipariş almak istemediğiniz durumlarda stok ve fiyat 
    /// bilginizi sıfırlamanız gerekmektedir.<br/>
    /// Her bir istek içerisinde gönderilebilecek maksimum item sayısı 1.000'dir.<br/>
    /// Onaylı ürünlerde productMainId değeri güncellenmemektedir.
    /// </summary>
    /// <param name="products"></param>
    /// <returns><see cref="bool"/></returns>
    [Obsolete("V2'de 4 uca bolundu: unapproved-bulk-update / content-bulk-update / " +
              "variant-bulk-update / delivery-info-bulk-update. Bu metot canli dogrulanmadi.")]
    public Task<bool> UpdateProductAsync(BulkModel<UpdateProductRequestModel> products);

    /// <summary>
    /// Trendyol'a aktarılan ve onaylanan ürünlerin fiyat ve stok bilgileri eş zamana yakın 
    /// güncellenir. Stok ve fiyat bligilerini istek içerisinde ayrı ayrı gönderebilirsiniz.<br/>
    /// Stok-fiyat güncelleme işlemlerinde request body içerisinde değişiklik yapmadan aynı isteği 
    /// tekrar atmanız halinde, sizlere hata mesajı dönecektir.Hata mesajı olarak "15 dakika boyunca
    /// aynı isteği tekrarlı olarak atamazsınız!" göreceksiniz.Sadece değişen stok-fiyatlarınızı 
    /// istek atacak şekilde sistemlerinizi düzeltmeniz gerekmektedir.<br/>
    /// Quantity alanında gönderdiğiniz stok, satılabilir stok bilgisidir.Satılabilir stok bilgisi
    /// sipariş alındığında ya da tarafınızdan yeniden stok gönderildiğinde güncellenir.<br/>
    /// Stok-fiyat update işlemlerinde maksimum 1000 item(sku) güncellemesi yapabilirsiniz.<br/>
    /// Ürünleriniz için maksimum 20 Bin adet stok ekleyebilirsiniz.
    /// </summary>
    /// <param name="products"></param>
    /// <returns><see cref="bool"/></returns>
    public Task<bool> UpdatePriceAndInventoryAsync(BulkModel<UpdateStockAndPriceRequestModel> products);

    /// <summary>
    /// Ürünleriniz Trendyol sisteminden kaldırırken bu metod kullanılmaktadır. 
    /// Tekli ve çoklu ürün silme işlemini desteklemektedir. Onay bekleyen 
    /// ürünlerinizi ve arşivde bir günden fazla bulunmuş, 
    /// Trendyol tarafından satışa durdurulmamış onaylı ürünlerinizi silebilirsiniz.
    /// </summary>
    /// <param name="products"></param>
    /// <returns><see cref="bool"/></returns>
    public Task<bool> DeleteProductsAsync(BulkModel<DeleteProductRequestModel> products);

    /// <summary>
    /// v2/createProducts, updatePriceAndInventory methodları servise yapılan istekler 
    /// kuyruğa atarak işlendiği için, servise yapılan her başarlı istek sonucunda 
    /// bir adet batchRequestId bilgisi dönülmektedir. Bu method yardımıyla batchRequestId 
    /// ile alınan işlemlerin sonucunun kontrolü yapılabilir. Servis dönüşündeki "status" 
    /// alanı kontrol edilerek toplu işlemin tamamlanıp tamamlanmadığı kontrol edilebilir. 
    /// Eğer toplu işlem sonucunda bir ya da birden fazla item için hata oluşmuş ise 
    /// failureReasons alanı kontrol edilerek sebebi bulunabilir.<br/>
    /// Ürün Aktarma ve Ürün Güncelleme servisleri, stok ve fiyat güncellemelerinde 
    /// sizlere dönen batch requesti 4 saat sonrasına kadar görüntüleyebilirsiniz.<br/>
    /// Stok Fiyat güncelleme işlemleri sonrasında sorguladığınız batchId için item bazlı 
    /// status alanlarını kontrol etmeniz gerekmektedir. Batch status alanı tarafınıza dönmeyecektir.
    /// </summary>
    /// <param name="batchRequestId"></param>
    /// <returns><see cref="bool"/></returns>
    public Task<GetBatchRequestResultResponseModel> GetBatchRequestResultAsync(string batchRequestId);
    
    /// <summary>
    /// V2 onaylı ürün filtreleme (.../products/approved). Nested response flat FilterProductsResponseModel'e
    /// düzleştirilir; NextPageToken ile token-based sayfalama yapılır.
    /// </summary>
    public Task<FilterProductsResponseModel?> FilterApprovedProductsAsync(string filterQuery);

    /// <summary>
    /// V2 onaysız ürün filtreleme (.../products/unapproved — pending/rejected). Flat modele düzleştirilir;
    /// NextPageToken ile token-based sayfalama yapılır.
    /// </summary>
    public Task<FilterProductsResponseModel?> FilterUnapprovedProductsAsync(string filterQuery);
}