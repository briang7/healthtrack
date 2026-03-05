using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using HealthTrack.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HealthTrack.Application.Common.Behaviors;

public sealed class AuditBehavior<TRequest, TResponse>(
    ICurrentUserService currentUserService,
    IAuditLogRepository auditLogRepository,
    ILogger<AuditBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (!requestName.Contains("Command", StringComparison.OrdinalIgnoreCase))
            return await next();

        var response = await next();

        try
        {
            var auditLog = new AuditLog
            {
                UserId = currentUserService.UserId ?? "system",
                Action = requestName,
                EntityType = requestName.Replace("Command", string.Empty),
                EntityId = ExtractEntityId(request),
                NewValues = PhiMaskingService.MaskJson(request),
                Timestamp = DateTime.UtcNow
            };

            await auditLogRepository.AddAsync(auditLog, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to create audit log for {RequestName}", requestName);
        }

        return response;
    }

    private static string ExtractEntityId(TRequest request)
    {
        var idProperty = typeof(TRequest).GetProperty("Id");
        return idProperty?.GetValue(request)?.ToString() ?? string.Empty;
    }
}
