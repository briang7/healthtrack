using HealthTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Persistence.Configurations;

public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
{
    public void Configure(EntityTypeBuilder<Pharmacy> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Fax)
            .HasMaxLength(20);

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

        builder.OwnsOne(p => p.Phone, phone =>
        {
            phone.Property(ph => ph.Number)
                .HasColumnName("Phone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.HasMany(p => p.Prescriptions)
            .WithOne(rx => rx.Pharmacy)
            .HasForeignKey(rx => rx.PharmacyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
