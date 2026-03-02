namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.Commands.BookAppointment;
using HealthTrack.Application.Features.Appointments.Commands.CancelAppointment;
using HealthTrack.Application.Features.Appointments.Commands.RescheduleAppointment;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Application.Features.Appointments.Queries.GetAppointmentById;
using HealthTrack.Application.Features.Appointments.Queries.GetAppointments;
using HealthTrack.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ISender _sender;

    public AppointmentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a filtered and paginated list of appointments.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AppointmentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? patientId = null,
        [FromQuery] Guid? providerId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] AppointmentStatus? status = null)
    {
        var query = new GetAppointmentsQuery(
            PatientId: patientId,
            ProviderId: providerId,
            DateFrom: fromDate,
            DateTo: toDate,
            Status: status,
            Page: page,
            PageSize: pageSize);
        var result = await _sender.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get an appointment by its unique identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetAppointmentByIdQuery(id));
        return Ok(result);
    }

    /// <summary>
    /// Book a new appointment.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Book([FromBody] BookAppointmentCommand command)
    {
        var result = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
    }

    /// <summary>
    /// Cancel an existing appointment.
    /// </summary>
    [HttpPatch("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelAppointmentRequest? request = null)
    {
        var result = await _sender.Send(new CancelAppointmentCommand(id, request?.Reason ?? "Cancelled by user."));
        return Ok(result);
    }

    /// <summary>
    /// Reschedule an existing appointment.
    /// </summary>
    [HttpPatch("{id:guid}/reschedule")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleAppointmentCommand command)
    {
        var updated = command with { Id = id };
        var result = await _sender.Send(updated);
        return Ok(result);
    }
}

public record CancelAppointmentRequest(string? Reason);
