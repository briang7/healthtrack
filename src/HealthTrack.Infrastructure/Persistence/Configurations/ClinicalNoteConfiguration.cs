using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class ClinicalNoteConfiguration : IEntityTypeConfiguration<ClinicalNote>
{
    public void Configure(EntityTypeBuilder<ClinicalNote> builder)
    {
        builder.HasKey(cn => cn.Id);

        builder.Property(cn => cn.NoteType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(cn => cn.Subjective)
            .HasMaxLength(4000);

        builder.Property(cn => cn.Objective)
            .HasMaxLength(4000);

        builder.Property(cn => cn.Assessment)
            .HasMaxLength(4000);

        builder.Property(cn => cn.Plan)
            .HasMaxLength(4000);

        builder.Property(cn => cn.Version)
            .HasDefaultValue(1);

        builder.HasOne(cn => cn.Patient)
            .WithMany(p => p.ClinicalNotes)
            .HasForeignKey(cn => cn.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cn => cn.Provider)
            .WithMany(p => p.ClinicalNotes)
            .HasForeignKey(cn => cn.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cn => cn.Appointment)
            .WithOne(a => a.ClinicalNote)
            .HasForeignKey<ClinicalNote>(cn => cn.AppointmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(cn => cn.PreviousVersion)
            .WithMany()
            .HasForeignKey(cn => cn.PreviousVersionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(cn => cn.PatientId);
        builder.HasIndex(cn => cn.ProviderId);
        builder.HasIndex(cn => cn.AppointmentId);
    }
}
