using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Audit.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Audit.Queries.GetAuditLogs;

public record GetAuditLogsQuery(
    string? UserId = null,
    string? EntityType = null,
    Guid? EntityId = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 20) : IRequest<ApiResponse<PagedResult<AuditLogDto>>>;
