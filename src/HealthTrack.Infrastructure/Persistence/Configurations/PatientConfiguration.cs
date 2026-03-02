using System.Text.Json;
using HealthTrack.Domain.Entities;
using HealthTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        builder.Property(p => p.Gender)
            .HasConversion<string>();

        builder.Property(p => p.MedicalHistory)
            .HasMaxLength(4000);

        builder.Property(p => p.Allergies)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>())
            .HasColumnType("jsonb");

        builder.Property(p => p.Medications)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>())
            .HasColumnType("jsonb");

        builder.OwnsOne(p => p.DateOfBirth, dob =>
        {
            dob.Property(d => d.Value)
                .HasColumnName("DateOfBirth")
                .IsRequired();
        });

        builder.OwnsOne(p => p.Phone, phone =>
        {
            phone.Property(ph => ph.Number)
                .HasColumnName("Phone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.OwnsOne(p => p.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Address_Street")
                .HasMaxLength(200)
                .IsRequired();

            address.Property(a => a.City)
                .HasColumnName("Address_City")
                .HasMaxLength(100)
                .IsRequired();

            address.Property(a => a.State)
                .HasColumnName("Address_State")
                .HasMaxLength(50)
                .IsRequired();

            address.Property(a => a.ZipCode)
                .HasColumnName("Address_ZipCode")
                .HasMaxLength(20)
                .IsRequired();

            address.Property(a => a.Country)
                .HasColumnName("Address_Country")
                .HasMaxLength(50)
                .HasDefaultValue("US");
        });

        builder.OwnsOne(p => p.InsuranceInfo, insurance =>
        {
            insurance.Property(i => i.Provider)
                .HasColumnName("Insurance_Provider")
                .HasMaxLength(200);

            insurance.Property(i => i.PolicyNumber)
                .HasColumnName("Insurance_PolicyNumber")
                .HasMaxLength(100);

            insurance.Property(i => i.GroupNumber)
                .HasColumnName("Insurance_GroupNumber")
                .HasMaxLength(100);

            insurance.Property(i => i.ExpirationDate)
                .HasColumnName("Insurance_ExpirationDate");
        });

        builder.OwnsOne(p => p.EmergencyContact, ec =>
        {
            ec.Property(e => e.Name)
                .HasColumnName("EmergencyContact_Name")
                .HasMaxLength(200);

            ec.Property(e => e.Relationship)
                .HasColumnName("EmergencyContact_Relationship")
                .HasMaxLength(100);

            ec.Property(e => e.Phone)
                .HasColumnName("EmergencyContact_Phone")
                .HasMaxLength(20);
        });

        builder.HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Prescriptions)
            .WithOne(rx => rx.Patient)
            .HasForeignKey(rx => rx.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ClinicalNotes)
            .WithOne(cn => cn.Patient)
            .HasForeignKey(cn => cn.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Consents)
            .WithOne(c => c.Patient)
            .HasForeignKey(c => c.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
