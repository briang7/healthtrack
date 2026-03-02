using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class ConsentConfiguration : IEntityTypeConfiguration<Consent>
{
    public void Configure(EntityTypeBuilder<Consent> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ConsentType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.GrantedAt)
            .IsRequired();

        builder.Property(c => c.DocumentUrl)
            .HasMaxLength(500);

        builder.HasOne(c => c.Patient)
            .WithMany(p => p.Consents)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.PatientId);
    }
}
