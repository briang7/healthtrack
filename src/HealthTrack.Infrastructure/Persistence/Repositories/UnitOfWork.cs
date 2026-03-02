using HealthTrack.Domain.Interfaces;

namespace HealthTrack.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IPatientRepository? _patients;
    private IProviderRepository? _providers;
    private IAppointmentRepository? _appointments;
    private IPrescriptionRepository? _prescriptions;
    private IClinicalNoteRepository? _clinicalNotes;
    private IAuditLogRepository? _auditLogs;
    private IPharmacyRepository? _pharmacies;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IPatientRepository Patients =>
        _patients ??= new PatientRepository(_context);

    public IProviderRepository Providers =>
        _providers ??= new ProviderRepository(_context);

    public IAppointmentRepository Appointments =>
        _appointments ??= new AppointmentRepository(_context);

    public IPrescriptionRepository Prescriptions =>
        _prescriptions ??= new PrescriptionRepository(_context);

    public IClinicalNoteRepository ClinicalNotes =>
        _clinicalNotes ??= new ClinicalNoteRepository(_context);

    public IAuditLogRepository AuditLogs =>
        _auditLogs ??= new AuditLogRepository(_context);

    public IPharmacyRepository Pharmacies =>
        _pharmacies ??= new PharmacyRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
