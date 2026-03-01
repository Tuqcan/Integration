using Integration.Marketplaces.Trendyol.Models.Provider;

namespace Integration.Marketplaces.Trendyol.Infrastructure.ProductIntegration.Constants;
public static class Providers
{
    public static List<GetProviderResponseModel> GetProviders()
    {
        return new List<GetProviderResponseModel>
        {
            new GetProviderResponseModel(38, "SENDEOMP", "Kolay Gelsin Marketplace", "2910804196"),
            new GetProviderResponseModel(30, "CEVATEDARIK", "Ceva Tedarik Marketplace", "1800038254"),
            new GetProviderResponseModel(10, "DHLECOMMP", "DHL eCommerce Marketplace", "6080712084"),
            new GetProviderResponseModel(19, "PTTMP", "PTT Kargo Marketplace", "7320068060"),
            new GetProviderResponseModel(9, "SURATMP", "Sürat Kargo Marketplace", "7870233582"),
            new GetProviderResponseModel(17, "TEXMP", "Trendyol Express Marketplace", "8590921777"),
            new GetProviderResponseModel(6, "HOROZMP", "Horoz Kargo Marketplace", "4630097122"),
            new GetProviderResponseModel(20, "CEVAMP", "CEVA Marketplace", "8450298557"),
            new GetProviderResponseModel(4, "YKMP", "Yurtiçi Kargo Marketplace", "3130557669"),
            new GetProviderResponseModel(7, "ARASMP", "Aras Kargo Marketplace", "720039666")
        };
    }

    public static GetProviderResponseModel? GetProviderById(int id) => GetProviders().FirstOrDefault(x => x.Id == id);
    public static GetProviderResponseModel? GetProviderByCode(string code) => GetProviders().FirstOrDefault(x => x.Code == code);
    public static GetProviderResponseModel? GetProviderByName(string name) => GetProviders().FirstOrDefault(x => x.Name == name);
}