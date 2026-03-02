using HealthTrack.Domain.Entities;

namespace HealthTrack.Domain.Interfaces;

public interface IProviderRepository : IRepository<Provider>
{
    Task<Provider?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Provider>> GetBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Provider>> GetAcceptingPatientsAsync(CancellationToken cancellationToken = default);
    Task<Provider?> GetWithAppointmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Provider?> GetWithSlotsAsync(Guid id, CancellationToken cancellationToken = default);
}
