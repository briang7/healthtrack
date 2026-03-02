using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class PharmacyRepository : GenericRepository<Pharmacy>, IPharmacyRepository
{
    public PharmacyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Pharmacy>> SearchByNameAsync(
        string name, CancellationToken cancellationToken = default)
    {
        var term = name.ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Name.ToLower().Contains(term))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Pharmacy>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }
}
