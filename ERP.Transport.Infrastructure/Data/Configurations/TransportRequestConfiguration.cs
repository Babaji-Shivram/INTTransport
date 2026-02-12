using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

public class TransportRequestConfiguration : BaseEntityConfiguration<TransportRequest>
{
    protected override string EntityName => "TransportRequest";

    public override void Configure(EntityTypeBuilder<TransportRequest> builder)
    {
        base.Configure(builder);

        builder.ToTable("TransportRequests");

        builder.Property(e => e.RequestNumber).HasMaxLength(30).IsRequired();
        builder.Property(e => e.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.GSTNumber).HasMaxLength(20);
        builder.Property(e => e.OriginLocationName).HasMaxLength(200);
        builder.Property(e => e.PickupAddress).HasMaxLength(500).IsRequired();
        builder.Property(e => e.PickupCity).HasMaxLength(100);
        builder.Property(e => e.PickupState).HasMaxLength(100);
        builder.Property(e => e.PickupPincode).HasMaxLength(10);
        builder.Property(e => e.DestinationLocationName).HasMaxLength(200);
        builder.Property(e => e.DropAddress).HasMaxLength(500).IsRequired();
        builder.Property(e => e.DropCity).HasMaxLength(100);
        builder.Property(e => e.DropState).HasMaxLength(100);
        builder.Property(e => e.DropPincode).HasMaxLength(10);
        builder.Property(e => e.CargoDescription).HasMaxLength(500);
        builder.Property(e => e.GrossWeightKg).HasColumnType("decimal(18,2)");
        builder.Property(e => e.SpecialInstructions).HasMaxLength(1000);
        builder.Property(e => e.BranchName).HasMaxLength(100);
        builder.Property(e => e.CountryCode).HasMaxLength(5).IsRequired();
        builder.Property(e => e.Division).HasMaxLength(50);
        builder.Property(e => e.Plant).HasMaxLength(100);
        builder.Property(e => e.WorkflowStatus).HasMaxLength(50);
        builder.Property(e => e.SourceReferenceNumber).HasMaxLength(50);
        builder.Property(e => e.RequestDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.RequiredDeliveryDate).HasColumnType("datetime2(7)");

        // Indexes
        builder.HasIndex(e => e.RequestNumber).IsUnique();
        builder.HasIndex(e => e.CustomerId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.BranchId);
        builder.HasIndex(e => e.CountryCode);
        builder.HasIndex(e => e.RequestDate);
        builder.HasIndex(e => e.SourceReferenceId);
        builder.HasIndex(e => e.ConsolidatedTripId);

        // Relationships
        builder.HasMany(e => e.Details)
            .WithOne(d => d.TransportRequest)
            .HasForeignKey(d => d.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Vehicles)
            .WithOne(v => v.TransportRequest)
            .HasForeignKey(v => v.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Movements)
            .WithOne(m => m.TransportRequest)
            .HasForeignKey(m => m.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Delivery)
            .WithOne(d => d.TransportRequest)
            .HasForeignKey<TransportDelivery>(d => d.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Documents)
            .WithOne(d => d.TransportRequest)
            .HasForeignKey(d => d.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Expenses)
            .WithOne(ex => ex.TransportRequest)
            .HasForeignKey(ex => ex.TransportRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
