namespace Integration.Hub;

/// <summary>
/// Pazaryeri 429 (Too Many Requests) donmeye devam etti ve tanimli deneme
/// hakki tukendi.
///
/// NEDEN AYRI BIR TIP: eskiden 429 dali "while(true)" icinde suresiz bekliyordu
/// ve transientRetries sayaci 429'da ARTMIYORDU. Yani limit kapanmazsa worker
/// sonsuza kadar asili kalir, heartbeat ise "calisiyor" derdi - servis-izleme
/// ekraninda hicbir sey yanlis gorunmezdi. Artik deneme hakki bitince YUKSEK
/// SESLE hata firlatiyoruz.
///
/// Sinif adi "RateLimit" gectigi icin ErrorSourceHelper bunu Trendyol API
/// kaynakli sayar ve "Sistem Hatasi" yerine dogru etiketle raporlar.
/// </summary>
public class RateLimitExceededException : Exception
{
    /// <summary>Limit asildigi icin denenen ve basarisiz olan istek sayisi.</summary>
    public int Attempts { get; }

    /// <summary>Limite takilan uc.</summary>
    public string Url { get; }

    public RateLimitExceededException(string url, int attempts)
        : base($"Trendyol istek limiti (429) {attempts} denemede acilmadi. URL: {url}")
    {
        Url = url;
        Attempts = attempts;
    }
}
