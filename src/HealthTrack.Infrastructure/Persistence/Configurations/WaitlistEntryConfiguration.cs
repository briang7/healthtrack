using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class WaitlistEntryConfiguration : IEntityTypeConfiguration<WaitlistEntry>
{
    public void Configure(EntityTypeBuilder<WaitlistEntry> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.RequestedDate)
            .IsRequired();

        builder.Property(w => w.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(w => w.Status)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(w => w.Patient)
            .WithMany()
            .HasForeignKey(w => w.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Provider)
            .WithMany()
            .HasForeignKey(w => w.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => new { w.PatientId, w.ProviderId });
    }
}
