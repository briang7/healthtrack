using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.MedicationName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Dosage)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Frequency)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Notes)
            .HasMaxLength(2000);

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.HasOne(p => p.Patient)
            .WithMany(pt => pt.Prescriptions)
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Provider)
            .WithMany(pr => pr.Prescriptions)
            .HasForeignKey(p => p.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Pharmacy)
            .WithMany(ph => ph.Prescriptions)
            .HasForeignKey(p => p.PharmacyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.PatientId);
        builder.HasIndex(p => p.ProviderId);
    }
}
