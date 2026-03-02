namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.Commands.CreatePatient;
using HealthTrack.Application.Features.Patients.Commands.DeletePatient;
using HealthTrack.Application.Features.Patients.Commands.UpdatePatient;
using HealthTrack.Application.Features.Patients.DTOs;
using HealthTrack.Application.Features.Patients.Queries.GetPatientById;
using HealthTrack.Application.Features.Patients.Queries.GetPatients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/patients")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly ISender _sender;

    public PatientsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a paginated list of patients.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var query = new GetPatientsQuery(
            Page: page,
            PageSize: pageSize,
            SearchTerm: search);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a patient by their unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PatientDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetPatientByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Create a new patient record.
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ProviderOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePatientCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Update an existing patient record.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "ProviderOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientCommand command)
    {
        var updated = command with { Id = id };
        var result = await _sender.Send(updated);
        return Ok(result);
    }

    /// <summary>
    /// Delete a patient record (admin only).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sender.Send(new DeletePatientCommand(id));
        return Ok(result);
    }
}
