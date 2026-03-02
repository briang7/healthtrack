using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class ClinicalNoteRepository : GenericRepository<ClinicalNote>, IClinicalNoteRepository
{
    public ClinicalNoteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ClinicalNote>> GetByPatientIdAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(cn => cn.Provider)
            .Where(cn => cn.PatientId == patientId)
            .OrderByDescending(cn => cn.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClinicalNote>> GetByProviderIdAsync(
        Guid providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(cn => cn.Patient)
            .Where(cn => cn.ProviderId == providerId)
            .OrderByDescending(cn => cn.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ClinicalNote?> GetByAppointmentIdAsync(
        Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(cn => cn.Provider)
            .Include(cn => cn.Patient)
            .FirstOrDefaultAsync(cn => cn.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<ClinicalNote>> GetVersionHistoryAsync(
        Guid noteId, CancellationToken cancellationToken = default)
    {
        var versions = new List<ClinicalNote>();
        var currentNote = await _dbSet
            .Include(cn => cn.Provider)
            .FirstOrDefaultAsync(cn => cn.Id == noteId, cancellationToken);

        while (currentNote is not null)
        {
            versions.Add(currentNote);

            if (currentNote.PreviousVersionId is null)
                break;

            currentNote = await _dbSet
                .Include(cn => cn.Provider)
                .FirstOrDefaultAsync(cn => cn.Id == currentNote.PreviousVersionId, cancellationToken);
        }

        return versions;
    }

    public async Task<ClinicalNote?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cn => cn.Patient)
            .Include(cn => cn.Provider)
            .Include(cn => cn.Appointment)
            .Include(cn => cn.PreviousVersion)
            .FirstOrDefaultAsync(cn => cn.Id == id, cancellationToken);
    }
}
