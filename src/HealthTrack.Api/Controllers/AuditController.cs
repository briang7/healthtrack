namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Audit.DTOs;
using HealthTrack.Application.Features.Audit.Queries.GetAuditLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/audit-logs")]
[Authorize(Policy = "AdminOnly")]
public class AuditController : ControllerBase
{
    private readonly ISender _sender;

    public AuditController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a paginated list of audit logs (admin only).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditLogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? userId = null,
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? entityId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var query = new GetAuditLogsQuery(
            UserId: userId,
            EntityType: entityType,
            EntityId: entityId,
            DateFrom: fromDate,
            DateTo: toDate,
            Page: page,
            PageSize: pageSize);
        var result = await _sender.Send(query);
        return Ok(result);
    }
}
