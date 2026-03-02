using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;

namespace HealthTrack.Domain.Interfaces;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByProviderIdAsync(Guid providerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Appointment>> GetUpcomingByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DateTime>> GetAvailableSlotsAsync(Guid providerId, DateTime date, CancellationToken cancellationToken = default);
}
