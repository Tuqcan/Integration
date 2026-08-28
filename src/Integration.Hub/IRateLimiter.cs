namespace Integration.Hub;

public interface IRateLimiter
{
    /// <summary>
    /// Rate limit kontrolu yapar. Limit asildiysa bekler, asilmadiysa devam eder.
    /// Asla hata firlatmaz.
    /// </summary>
    Task WaitAsync(string category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Supplier bazli rate limit kontrolu yapar. Her supplier icin ayri pencere tutar.
    /// Limit asildiysa bekler, asilmadiysa devam eder. Asla hata firlatmaz.
    /// </summary>
    Task WaitAsync(string category, int supplierId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pazaryeri 429 dondurdugunde cagrilir: "bu hesap icin tahminim YUKSEKMIS".
    ///
    /// Tier artik saticinin listeleme kotasindan CIKARILIYOR
    /// (bkz. TrendyolRateLimitConfig.InferTier) ve bu bir TAHMINDIR. Tahmin fazla
    /// genis cikarsa tek belirti 429'dur; geri bildirim olmadan sistem ayni yanlis
    /// limitle dovmeye devam eder. Bu metot uygulayiciya cikarimi geri alma firsati verir.
    ///
    /// Varsayilan uygulama HICBIR SEY YAPMAZ: geri bildirim opsiyonel bir yetenektir ve
    /// mevcut uygulayicilari kirmaz. Asla hata firlatmamalidir - 429 zaten sikintili bir
    /// yol, bir de izleme kodu yuzunden patlamamali.
    /// </summary>
    Task ReportRateLimitedAsync(string category, int supplierId, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
