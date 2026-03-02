using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Queries.GetPatientById;

public record GetPatientByIdQuery(Guid Id) : IRequest<ApiResponse<PatientDetailDto>>;
