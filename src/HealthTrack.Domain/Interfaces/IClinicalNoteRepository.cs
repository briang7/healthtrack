using HealthTrack.Domain.Entities;

namespace HealthTrack.Domain.Interfaces;

public interface IClinicalNoteRepository : IRepository<ClinicalNote>
{
    Task<IReadOnlyList<ClinicalNote>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClinicalNote>> GetByProviderIdAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<ClinicalNote?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClinicalNote>> GetVersionHistoryAsync(Guid noteId, CancellationToken cancellationToken = default);
    Task<ClinicalNote?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
