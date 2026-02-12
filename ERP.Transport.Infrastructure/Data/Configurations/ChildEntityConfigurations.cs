using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

public class TransportRequestDetailConfiguration : BaseEntityConfiguration<TransportRequestDetail>
{
    protected override string EntityName => "TransportRequestDetail";

    public override void Configure(EntityTypeBuilder<TransportRequestDetail> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportRequestDetails");

        builder.Property(e => e.FieldKey).HasMaxLength(100).IsRequired();
        builder.Property(e => e.FieldLabel).HasMaxLength(200);
        builder.Property(e => e.FieldValue).HasMaxLength(2000);
        builder.Property(e => e.DataType).HasMaxLength(20);

        builder.HasIndex(e => new { e.TransportRequestId, e.FieldKey });
    }
}

public class TransportVehicleConfiguration : BaseEntityConfiguration<TransportVehicle>
{
    protected override string EntityName => "TransportVehicle";

    public override void Configure(EntityTypeBuilder<TransportVehicle> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportVehicles");

        builder.Property(e => e.TransporterName).HasMaxLength(200);
        builder.Property(e => e.VehicleNumber).HasMaxLength(20).IsRequired();
        builder.Property(e => e.DriverName).HasMaxLength(100);
        builder.Property(e => e.DriverPhone).HasMaxLength(20);
        builder.Property(e => e.LRNumber).HasMaxLength(50);
        builder.Property(e => e.LRDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.MemoCopyUrl).HasMaxLength(500);
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.HasIndex(e => e.TransportRequestId);
        builder.HasIndex(e => e.TransporterId);

        builder.HasOne(e => e.Rate)
            .WithOne(r => r.TransportVehicle)
            .HasForeignKey<VehicleRate>(r => r.TransportVehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.FundRequest)
            .WithOne(f => f.TransportVehicle)
            .HasForeignKey<VehicleFundRequest>(f => f.TransportVehicleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Transporter)
            .WithMany(t => t.VehicleAssignments)
            .HasForeignKey(e => e.TransporterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class VehicleRateConfiguration : BaseEntityConfiguration<VehicleRate>
{
    protected override string EntityName => "VehicleRate";

    public override void Configure(EntityTypeBuilder<VehicleRate> builder)
    {
        base.Configure(builder);
        builder.ToTable("VehicleRates");

        builder.Property(e => e.FreightRate).HasColumnType("decimal(18,2)");
        builder.Property(e => e.DetentionCharges).HasColumnType("decimal(18,2)");
        builder.Property(e => e.VaraiCharges).HasColumnType("decimal(18,2)");
        builder.Property(e => e.EmptyContainerReturn).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TollCharges).HasColumnType("decimal(18,2)");
        builder.Property(e => e.OtherCharges).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalRate).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("INR");
        builder.Property(e => e.ApprovedAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.ApprovalRemarks).HasMaxLength(500);
        builder.Property(e => e.ApprovedDate).HasColumnType("datetime2(7)");

        builder.HasIndex(e => e.TransportVehicleId).IsUnique();
    }
}

public class VehicleFundRequestConfiguration : BaseEntityConfiguration<VehicleFundRequest>
{
    protected override string EntityName => "VehicleFundRequest";

    public override void Configure(EntityTypeBuilder<VehicleFundRequest> builder)
    {
        base.Configure(builder);
        builder.ToTable("VehicleFundRequests");

        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("INR");
        builder.Property(e => e.BankName).HasMaxLength(200);
        builder.Property(e => e.AccountNumber).HasMaxLength(30);
        builder.Property(e => e.IFSCCode).HasMaxLength(15);
        builder.Property(e => e.Remarks).HasMaxLength(500);
        builder.Property(e => e.ProcessedDate).HasColumnType("datetime2(7)");

        builder.HasIndex(e => e.TransportVehicleId).IsUnique();
    }
}
