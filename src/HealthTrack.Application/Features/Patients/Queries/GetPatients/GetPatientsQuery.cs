using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Patients.DTOs;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Queries.GetPatients;

public record GetPatientsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IRequest<ApiResponse<PagedResult<PatientDto>>>;
