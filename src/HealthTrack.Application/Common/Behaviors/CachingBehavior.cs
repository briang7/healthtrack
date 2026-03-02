using HealthTrack.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HealthTrack.Application.Common.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(
    ICacheService cacheService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICacheableQuery cacheableQuery)
            return await next();

        var cacheKey = cacheableQuery.CacheKey;

        var cachedResult = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
        if (cachedResult is not null)
        {
            logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cachedResult;
        }

        logger.LogInformation("Cache miss for {CacheKey}, executing handler", cacheKey);

        var response = await next();

        await cacheService.SetAsync(
            cacheKey,
            response,
            cacheableQuery.Expiration,
            cancellationToken);

        return response;
    }
}
