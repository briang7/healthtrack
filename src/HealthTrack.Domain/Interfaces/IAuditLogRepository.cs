using HealthTrack.Domain.Entities;

namespace HealthTrack.Domain.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default);
}
