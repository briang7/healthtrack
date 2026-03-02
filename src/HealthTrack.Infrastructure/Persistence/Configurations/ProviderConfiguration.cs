using HealthTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Specialty)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.LicenseNumber)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(p => p.LicenseNumber)
            .IsUnique();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.OwnsOne(p => p.Phone, phone =>
        {
            phone.Property(ph => ph.Number)
                .HasColumnName("Phone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Provider)
            .HasForeignKey(a => a.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Prescriptions)
            .WithOne(rx => rx.Provider)
            .HasForeignKey(rx => rx.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ClinicalNotes)
            .WithOne(cn => cn.Provider)
            .HasForeignKey(cn => cn.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.AppointmentSlots)
            .WithOne(s => s.Provider)
            .HasForeignKey(s => s.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
