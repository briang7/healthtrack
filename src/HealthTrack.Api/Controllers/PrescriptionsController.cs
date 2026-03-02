namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Prescriptions.Commands.CheckDrugInteractions;
using HealthTrack.Application.Features.Prescriptions.Commands.CreatePrescription;
using HealthTrack.Application.Features.Prescriptions.Commands.RequestRefill;
using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Features.Prescriptions.DTOs;
using HealthTrack.Application.Features.Prescriptions.Queries.GetPrescriptionById;
using HealthTrack.Application.Features.Prescriptions.Queries.GetPrescriptions;
using HealthTrack.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/prescriptions")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly ISender _sender;

    public PrescriptionsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a paginated list of prescriptions.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PrescriptionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? patientId = null,
        [FromQuery] PrescriptionStatus? status = null)
    {
        var query = new GetPrescriptionsQuery(
            PatientId: patientId,
            Status: status,
            Page: page,
            PageSize: pageSize);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a prescription by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PrescriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetPrescriptionByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Create a new prescription (provider or admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "ProviderOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePrescriptionCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Request a refill for an existing prescription.
    /// </summary>
    [HttpPost("{id:guid}/refill")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RequestRefill(Guid id)
    {
        var result = await _sender.Send(new RequestRefillCommand(id));
        return Ok(result);
    }

    /// <summary>
    /// Check for drug interactions between medications.
    /// </summary>
    [HttpPost("check-interactions")]
    [ProducesResponseType(typeof(ApiResponse<List<DrugInteraction>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckInteractions([FromBody] CheckDrugInteractionsCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
}
