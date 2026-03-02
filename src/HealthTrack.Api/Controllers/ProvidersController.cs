namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Providers.Commands.CreateProvider;
using HealthTrack.Application.Features.Providers.Commands.UpdateProvider;
using HealthTrack.Application.Features.Providers.DTOs;
using HealthTrack.Application.Features.Providers.Queries.GetProviderById;
using HealthTrack.Application.Features.Providers.Queries.GetProviders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/providers")]
[Authorize]
public class ProvidersController : ControllerBase
{
    private readonly ISender _sender;

    public ProvidersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a paginated list of providers.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProviderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? specialty = null,
        [FromQuery] string? search = null)
    {
        var query = new GetProvidersQuery(
            Specialty: specialty,
            AcceptingPatients: null,
            Page: page,
            PageSize: pageSize);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a provider by their unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProviderDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetProviderByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Create a new provider (admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProviderCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update an existing provider (admin only).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProviderCommand command)
    {
        var updated = command with { Id = id };
        var result = await _sender.Send(updated);
        return Ok(result);
    }
}
