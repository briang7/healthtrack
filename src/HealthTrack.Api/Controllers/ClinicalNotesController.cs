namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.ClinicalNotes.Commands.AmendClinicalNote;
using HealthTrack.Application.Features.ClinicalNotes.Commands.CreateClinicalNote;
using HealthTrack.Application.Features.ClinicalNotes.DTOs;
using HealthTrack.Application.Features.ClinicalNotes.Queries.GetClinicalNoteById;
using HealthTrack.Application.Features.ClinicalNotes.Queries.GetClinicalNotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/clinical-notes")]
[Authorize]
public class ClinicalNotesController : ControllerBase
{
    private readonly ISender _sender;

    public ClinicalNotesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a paginated list of clinical notes.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClinicalNoteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? providerId = null)
    {
        var query = new GetClinicalNotesQuery(
            PatientId: patientId,
            ProviderId: providerId,
            Page: page,
            PageSize: pageSize);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a clinical note by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClinicalNoteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetClinicalNoteByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Create a new clinical note (provider or admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ProviderOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateClinicalNoteCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Amend an existing clinical note (provider or admin only).
    /// </summary>
    [HttpPut("{id:guid}/amend")]
    [Authorize(Policy = "ProviderOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Amend(Guid id, [FromBody] AmendClinicalNoteCommand command)
    {
        var updated = command with { Id = id };
        var result = await _sender.Send(updated);
        return Ok(result);
    }
}
