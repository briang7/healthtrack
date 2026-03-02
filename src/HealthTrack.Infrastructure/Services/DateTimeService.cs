using HealthTrack.Application.Common.Interfaces;

namespace HealthTrack.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.UtcNow;
}
