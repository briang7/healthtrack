using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using HealthTrack.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Provider)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByProviderIdAsync(
        Guid providerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Patient)
            .Where(a => a.ProviderId == providerId)
            .OrderByDescending(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(
        DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Provider)
            .Where(a => a.ScheduledAt >= start && a.ScheduledAt <= end)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetByStatusAsync(
        AppointmentStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Patient)
            .Include(a => a.Provider)
            .Where(a => a.Status == status)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> GetUpcomingByPatientIdAsync(
        Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(a => a.Provider)
            .Where(a => a.PatientId == patientId &&
                        a.ScheduledAt > DateTime.UtcNow &&
                        a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Provider)
            .Include(a => a.ClinicalNote)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DateTime>> GetAvailableSlotsAsync(
        Guid providerId, DateTime date, CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        var bookedSlots = await _dbSet
            .AsNoTracking()
            .Where(a => a.ProviderId == providerId &&
                        a.ScheduledAt >= startOfDay &&
                        a.ScheduledAt < endOfDay &&
                        a.Status != AppointmentStatus.Cancelled)
            .Select(a => a.ScheduledAt)
            .ToListAsync(cancellationToken);

        var allSlots = new List<DateTime>();
        var slotStart = startOfDay.AddHours(8); // 8 AM
        var slotEnd = startOfDay.AddHours(17);  // 5 PM

        while (slotStart < slotEnd)
        {
            if (!bookedSlots.Contains(slotStart))
                allSlots.Add(slotStart);
            slotStart = slotStart.AddMinutes(30);
        }

        return allSlots;
    }
}
