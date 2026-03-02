namespace HealthTrack.Application.Common.Behaviors;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration { get; }
}
