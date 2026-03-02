using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class ProviderRepository : GenericRepository<Provider>, IProviderRepository
{
    public ProviderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Provider?> GetByLicenseNumberAsync(
        string licenseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.LicenseNumber == licenseNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Provider>> GetBySpecialtyAsync(
        string specialty, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.Specialty == specialty)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Provider>> GetAcceptingPatientsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.IsAcceptingPatients)
            .OrderBy(p => p.Specialty)
            .ThenBy(p => p.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Provider?> GetWithAppointmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Patient)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Provider?> GetWithSlotsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.AppointmentSlots.Where(s => s.StartTime >= DateTime.UtcNow))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
