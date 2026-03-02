using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Audit.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Audit.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler(
    IAuditLogRepository auditLogRepository,
    IMapper mapper)
    : IRequestHandler<GetAuditLogsQuery, ApiResponse<PagedResult<AuditLogDto>>>
{
    public async Task<ApiResponse<PagedResult<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AuditLog> auditLogs;

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            auditLogs = await auditLogRepository.GetByUserAsync(request.UserId, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.EntityType) && request.EntityId.HasValue)
        {
            auditLogs = await auditLogRepository.GetByEntityAsync(
                request.EntityType, request.EntityId.Value, cancellationToken);
        }
        else if (request.DateFrom.HasValue && request.DateTo.HasValue)
        {
            auditLogs = await auditLogRepository.GetByDateRangeAsync(
                request.DateFrom.Value, request.DateTo.Value, cancellationToken);
        }
        else
        {
            auditLogs = await auditLogRepository.GetByDateRangeAsync(
                DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, cancellationToken);
        }

        var filtered = auditLogs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.EntityType) && string.IsNullOrWhiteSpace(request.UserId))
            filtered = filtered.Where(a => a.EntityType == request.EntityType);

        if (request.DateFrom.HasValue)
            filtered = filtered.Where(a => a.Timestamp >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            filtered = filtered.Where(a => a.Timestamp <= request.DateTo.Value);

        var filteredList = filtered.OrderByDescending(a => a.Timestamp).ToList();
        var totalCount = filteredList.Count;

        var paged = filteredList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<AuditLogDto>>(paged);

        var result = new PagedResult<AuditLogDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<AuditLogDto>>.SuccessResponse(result);
    }
}
