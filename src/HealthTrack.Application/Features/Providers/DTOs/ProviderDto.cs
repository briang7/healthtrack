namespace HealthTrack.Application.Features.Providers.DTOs;

public record ProviderDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Specialty { get; init; } = string.Empty;
    public bool IsAcceptingPatients { get; init; }
}
