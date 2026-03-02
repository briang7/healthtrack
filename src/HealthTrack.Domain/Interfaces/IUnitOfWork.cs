namespace HealthTrack.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPatientRepository Patients { get; }
    IAppointmentRepository Appointments { get; }
    IPrescriptionRepository Prescriptions { get; }
    IClinicalNoteRepository ClinicalNotes { get; }
    IProviderRepository Providers { get; }
    IAuditLogRepository AuditLogs { get; }
    IPharmacyRepository Pharmacies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
