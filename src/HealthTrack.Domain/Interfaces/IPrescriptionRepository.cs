using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Interfaces;

public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<IReadOnlyList<Prescription>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetByProviderIdAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetActivePrescriptionsAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Prescription>> GetRefillRequestsAsync(CancellationToken cancellationToken = default);
    Task<Prescription?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
