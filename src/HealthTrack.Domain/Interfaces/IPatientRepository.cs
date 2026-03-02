using HealthTrack.Domain.Entities;

namespace HealthTrack.Domain.Interfaces;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Patient?> GetWithAppointmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetWithPrescriptionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Patient?> GetWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
