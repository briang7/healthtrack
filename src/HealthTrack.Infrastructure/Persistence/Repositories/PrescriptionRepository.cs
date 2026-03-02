using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class PrescriptionRepository : GenericRepository<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Prescription>> GetByPatientIdAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rx => rx.Provider)
            .Include(rx => rx.Pharmacy)
            .Where(rx => rx.PatientId == patientId)
            .OrderByDescending(rx => rx.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetActivePrescriptionsAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rx => rx.Provider)
            .Include(rx => rx.Pharmacy)
            .Where(rx => rx.PatientId == patientId && rx.Status == PrescriptionStatus.Active)
            .OrderByDescending(rx => rx.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetRefillRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rx => rx.Patient)
            .Include(rx => rx.Provider)
            .Include(rx => rx.Pharmacy)
            .Where(rx => rx.Status == PrescriptionStatus.RefillRequested)
            .OrderBy(rx => rx.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetByProviderIdAsync(
        Guid providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rx => rx.Patient)
            .Include(rx => rx.Pharmacy)
            .Where(rx => rx.ProviderId == providerId)
            .OrderByDescending(rx => rx.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetActiveByPatientIdAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        return await GetActivePrescriptionsAsync(patientId, cancellationToken);
    }

    public async Task<IReadOnlyList<Prescription>> GetByStatusAsync(
        PrescriptionStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(rx => rx.Patient)
            .Include(rx => rx.Provider)
            .Where(rx => rx.Status == status)
            .OrderByDescending(rx => rx.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Prescription?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(rx => rx.Patient)
            .Include(rx => rx.Provider)
            .Include(rx => rx.Pharmacy)
            .FirstOrDefaultAsync(rx => rx.Id == id, cancellationToken);
    }
}
