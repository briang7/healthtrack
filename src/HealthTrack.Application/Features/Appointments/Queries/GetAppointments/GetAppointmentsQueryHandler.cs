using AutoMapper;
using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Appointments.DTOs;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using MediatR;

namespace HealthTrack.Application.Features.Appointments.Queries.GetAppointments;

public sealed class GetAppointmentsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : IRequestHandler<GetAppointmentsQuery, ApiResponse<PagedResult<AppointmentDto>>>
{
    public async Task<ApiResponse<PagedResult<AppointmentDto>>> Handle(
        GetAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Appointment> appointments;

        if (request.PatientId.HasValue)
        {
            appointments = await unitOfWork.Appointments
                .GetByPatientIdAsync(request.PatientId.Value, cancellationToken);
        }
        else if (request.ProviderId.HasValue)
        {
            appointments = await unitOfWork.Appointments
                .GetByProviderIdAsync(request.ProviderId.Value, cancellationToken);
        }
        else if (request.DateFrom.HasValue && request.DateTo.HasValue)
        {
            appointments = await unitOfWork.Appointments
                .GetByDateRangeAsync(request.DateFrom.Value, request.DateTo.Value, cancellationToken);
        }
        else
        {
            appointments = await unitOfWork.Appointments.GetAllAsync(cancellationToken);
        }

        var filtered = appointments.AsEnumerable();

        if (request.Status.HasValue)
            filtered = filtered.Where(a => a.Status == request.Status.Value);

        if (request.DateFrom.HasValue)
            filtered = filtered.Where(a => a.ScheduledAt >= request.DateFrom.Value);

        if (request.DateTo.HasValue)
            filtered = filtered.Where(a => a.ScheduledAt <= request.DateTo.Value);

        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;

        var paged = filteredList
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var dtos = mapper.Map<List<AppointmentDto>>(paged);

        var result = new PagedResult<AppointmentDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResult<AppointmentDto>>.SuccessResponse(result);
    }
}
