using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

// ═══════════════════════════════════════════════════════════════
//  Phase 3: Maintenance & Travel Log configurations only.
//  (FleetVehicle, VehicleDriver, VehicleDailyStatus,
//   ConsolidatedTrip were created in Phase 2 without configs —
//   adding configs now would generate ALTER statements.)
// ═══════════════════════════════════════════════════════════════

public class MaintenanceWorkOrderConfiguration : BaseEntityConfiguration<MaintenanceWorkOrder>
{
    protected override string EntityName => "MaintenanceWorkOrder";

    public override void Configure(EntityTypeBuilder<MaintenanceWorkOrder> builder)
    {
        base.Configure(builder);
        builder.ToTable("MaintenanceWorkOrders");

        builder.Property(e => e.WorkOrderNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(e => e.WorkOrderNumber).IsUnique();

        builder.Property(e => e.EstimatedCost).HasPrecision(18, 2);
        builder.Property(e => e.ActualCost).HasPrecision(18, 2);
        builder.Property(e => e.LaborCost).HasPrecision(18, 2);
        builder.Property(e => e.PartsCost).HasPrecision(18, 2);
        builder.Property(e => e.OdometerAtService).HasPrecision(18, 2);
        builder.Property(e => e.NextServiceOdometer).HasPrecision(18, 2);

        builder.HasIndex(e => e.FleetVehicleId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.ScheduledDate);
        builder.HasIndex(e => new { e.CountryCode, e.BranchId });
    }
}

public class MaintenancePartConfiguration : BaseEntityConfiguration<MaintenancePart>
{
    protected override string EntityName => "MaintenancePart";

    public override void Configure(EntityTypeBuilder<MaintenancePart> builder)
    {
        base.Configure(builder);
        builder.ToTable("MaintenanceParts");

        builder.Property(e => e.UnitCost).HasPrecision(18, 2);
        builder.Property(e => e.TotalCost).HasPrecision(18, 2);

        builder.HasIndex(e => e.MaintenanceWorkOrderId);
    }
}

public class MaintenanceDocumentConfiguration : BaseEntityConfiguration<MaintenanceDocument>
{
    protected override string EntityName => "MaintenanceDocument";

    public override void Configure(EntityTypeBuilder<MaintenanceDocument> builder)
    {
        base.Configure(builder);
        builder.ToTable("MaintenanceDocuments");

        builder.Property(e => e.FileName).HasMaxLength(500).IsRequired();
        builder.Property(e => e.FileUrl).HasMaxLength(2000).IsRequired();
        builder.Property(e => e.ContentType).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.Remarks).HasMaxLength(1000);

        builder.HasIndex(e => e.MaintenanceWorkOrderId);
        builder.HasIndex(e => e.DocumentType);

        builder.HasOne(e => e.MaintenanceWorkOrder)
            .WithMany(w => w.Documents)
            .HasForeignKey(e => e.MaintenanceWorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class VehicleTravelLogConfiguration : BaseEntityConfiguration<VehicleTravelLog>
{
    protected override string EntityName => "VehicleTravelLog";

    public override void Configure(EntityTypeBuilder<VehicleTravelLog> builder)
    {
        base.Configure(builder);
        builder.ToTable("VehicleTravelLogs");

        builder.Property(e => e.StartOdometerKm).HasPrecision(18, 2);
        builder.Property(e => e.EndOdometerKm).HasPrecision(18, 2);
        builder.Property(e => e.DistanceKm).HasPrecision(18, 2);
        builder.Property(e => e.FuelConsumedLitres).HasPrecision(10, 2);
        builder.Property(e => e.FuelCost).HasPrecision(18, 2);
        builder.Property(e => e.TollCharges).HasPrecision(18, 2);
        builder.Property(e => e.ParkingCharges).HasPrecision(18, 2);
        builder.Property(e => e.OtherExpenses).HasPrecision(18, 2);

        builder.HasIndex(e => e.FleetVehicleId);
        builder.HasIndex(e => e.TripDate);
        builder.HasIndex(e => e.TransportRequestId);
    }
}
