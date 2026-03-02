using HealthTrack.Application.Common.Models;
using MediatR;

namespace HealthTrack.Application.Features.Patients.Commands.DeletePatient;

public record DeletePatientCommand(Guid Id) : IRequest<ApiResponse<bool>>;
