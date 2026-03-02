using Microsoft.AspNetCore.Identity;

namespace HealthTrack.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Patient";
    public Guid? LinkedPatientId { get; set; }
    public Guid? LinkedProviderId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
