using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

// ═══════════════════════════════════════════════════════════════
//  Consolidated Trip sub-entity configurations:
//  ConsolidatedVehicle, ConsolidatedExpense, ConsolidatedStopDelivery
// ═══════════════════════════════════════════════════════════════

public class ConsolidatedVehicleConfiguration : BaseEntityConfiguration<ConsolidatedVehicle>
{
    protected override string EntityName => "ConsolidatedVehicle";

    public override void Configure(EntityTypeBuilder<ConsolidatedVehicle> builder)
    {
        base.Configure(builder);
        builder.ToTable("ConsolidatedVehicles");

        builder.Property(e => e.VehicleNumber).HasMaxLength(20).IsRequired();
        builder.Property(e => e.TransporterName).HasMaxLength(200);
        builder.Property(e => e.DriverName).HasMaxLength(150);
        builder.Property(e => e.DriverPhone).HasMaxLength(20);
        builder.Property(e => e.CurrencyCode).HasMaxLength(5).HasDefaultValue("INR");
        builder.Property(e => e.LRNumber).HasMaxLength(50);
        builder.Property(e => e.MemoCopyUrl).HasMaxLength(500);

        builder.Property(e => e.FreightRate).HasPrecision(18, 2);
        builder.Property(e => e.TollCharges).HasPrecision(18, 2);
        builder.Property(e => e.OtherCharges).HasPrecision(18, 2);
        builder.Property(e => e.TotalRate).HasPrecision(18, 2);

        builder.HasIndex(e => e.ConsolidatedTripId);
        builder.HasIndex(e => e.TransporterId);
        builder.HasIndex(e => e.VehicleNumber);
    }
}

public class ConsolidatedExpenseConfiguration : BaseEntityConfiguration<ConsolidatedExpense>
{
    protected override string EntityName => "ConsolidatedExpense";

    public override void Configure(EntityTypeBuilder<ConsolidatedExpense> builder)
    {
        base.Configure(builder);
        builder.ToTable("ConsolidatedExpenses");

        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.CurrencyCode).HasMaxLength(5).HasDefaultValue("INR");
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.ReceiptUrl).HasMaxLength(500);
        builder.Property(e => e.Remarks).HasMaxLength(1000);

        builder.HasIndex(e => e.ConsolidatedTripId);
        builder.HasIndex(e => e.ExpenseDate);
    }
}

public class ConsolidatedStopDeliveryConfiguration : BaseEntityConfiguration<ConsolidatedStopDelivery>
{
    protected override string EntityName => "ConsolidatedStopDelivery";

    public override void Configure(EntityTypeBuilder<ConsolidatedStopDelivery> builder)
    {
        base.Configure(builder);
        builder.ToTable("ConsolidatedStopDeliveries");

        builder.Property(e => e.LocationName).HasMaxLength(200);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.Pincode).HasMaxLength(10);
        builder.Property(e => e.ReceivedBy).HasMaxLength(200);
        builder.Property(e => e.PODNumber).HasMaxLength(50);
        builder.Property(e => e.PODDocumentUrl).HasMaxLength(500);
        builder.Property(e => e.Remarks).HasMaxLength(1000);

        builder.HasIndex(e => e.ConsolidatedTripId);
        builder.HasIndex(e => e.TransportRequestId);
        builder.HasIndex(e => new { e.ConsolidatedTripId, e.StopSequence });
    }
}
