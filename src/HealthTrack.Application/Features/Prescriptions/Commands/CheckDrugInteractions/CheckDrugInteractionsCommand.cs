using HealthTrack.Application.Common.Interfaces;
using HealthTrack.Application.Common.Models;
using MediatR;

namespace HealthTrack.Application.Features.Prescriptions.Commands.CheckDrugInteractions;

public record CheckDrugInteractionsCommand(
    Guid PatientId,
    string Medication) : IRequest<ApiResponse<List<DrugInteraction>>>;
