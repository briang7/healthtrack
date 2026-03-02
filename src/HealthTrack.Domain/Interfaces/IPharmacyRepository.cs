using HealthTrack.Domain.Entities;

namespace HealthTrack.Domain.Interfaces;

public interface IPharmacyRepository : IRepository<Pharmacy>
{
    Task<IReadOnlyList<Pharmacy>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Pharmacy>> GetActiveAsync(CancellationToken cancellationToken = default);
}
