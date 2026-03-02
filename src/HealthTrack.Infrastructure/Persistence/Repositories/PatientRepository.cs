using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var term = searchTerm.ToLower();
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.FirstName.ToLower().Contains(term) ||
                        p.LastName.ToLower().Contains(term) ||
                        p.Email.ToLower().Contains(term))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Patient?> GetWithAppointmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Provider)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Patient?> GetWithPrescriptionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Prescriptions)
                .ThenInclude(rx => rx.Provider)
            .Include(p => p.Prescriptions)
                .ThenInclude(rx => rx.Pharmacy)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Patient?> GetWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Provider)
            .Include(p => p.Prescriptions)
                .ThenInclude(rx => rx.Provider)
            .Include(p => p.Prescriptions)
                .ThenInclude(rx => rx.Pharmacy)
            .Include(p => p.ClinicalNotes)
                .ThenInclude(cn => cn.Provider)
            .Include(p => p.Consents)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
