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
}
