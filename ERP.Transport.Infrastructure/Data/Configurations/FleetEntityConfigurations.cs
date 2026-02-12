using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

public class FleetVehicleConfiguration : IEntityTypeConfiguration<FleetVehicle>
{
    public void Configure(EntityTypeBuilder<FleetVehicle> builder)
    {
        builder.Property(v => v.LoadCapacityKg).HasPrecision(18, 2);
        builder.Property(v => v.VolumeCapacityCBM).HasPrecision(18, 4);
        builder.Property(v => v.CurrentOdometerKm).HasPrecision(18, 2);
        builder.HasIndex(v => v.RegistrationNumber).IsUnique();
    }
}

public class VehicleDailyStatusConfiguration : IEntityTypeConfiguration<VehicleDailyStatus>
{
    public void Configure(EntityTypeBuilder<VehicleDailyStatus> builder)
    {
        builder.Property(s => s.OdometerKm).HasPrecision(18, 2);
        builder.HasIndex(s => new { s.FleetVehicleId, s.Date });
    }
}

public class ConsolidatedTripConfiguration : IEntityTypeConfiguration<ConsolidatedTrip>
{
    public void Configure(EntityTypeBuilder<ConsolidatedTrip> builder)
    {
        builder.HasIndex(t => t.ReferenceNumber).IsUnique();
        builder.HasMany(t => t.Jobs)
            .WithOne(j => j.ConsolidatedTrip)
            .HasForeignKey(j => j.ConsolidatedTripId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
