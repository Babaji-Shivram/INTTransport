using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

public class TransportMovementConfiguration : BaseEntityConfiguration<TransportMovement>
{
    protected override string EntityName => "TransportMovement";

    public override void Configure(EntityTypeBuilder<TransportMovement> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportMovements");

        builder.Property(e => e.LocationName).HasMaxLength(200);
        builder.Property(e => e.Latitude).HasColumnType("decimal(10,7)");
        builder.Property(e => e.Longitude).HasColumnType("decimal(10,7)");
        builder.Property(e => e.Timestamp).HasColumnType("datetime2(7)");
        builder.Property(e => e.Remarks).HasMaxLength(500);

        builder.HasIndex(e => e.TransportRequestId);
        builder.HasIndex(e => e.Timestamp);
    }
}

public class TransportDeliveryConfiguration : BaseEntityConfiguration<TransportDelivery>
{
    protected override string EntityName => "TransportDelivery";

    public override void Configure(EntityTypeBuilder<TransportDelivery> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportDeliveries");

        builder.Property(e => e.DeliveryDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.ReceivedBy).HasMaxLength(200).IsRequired();
        builder.Property(e => e.PODNumber).HasMaxLength(50);
        builder.Property(e => e.PODDocumentUrl).HasMaxLength(500);
        builder.Property(e => e.ChallanNumber).HasMaxLength(50);
        builder.Property(e => e.ChallanDocumentUrl).HasMaxLength(500);
        builder.Property(e => e.LRCopyUrl).HasMaxLength(500);
        builder.Property(e => e.EWayBillNumber).HasMaxLength(30);
        builder.Property(e => e.DamageNotes).HasMaxLength(1000);
        builder.Property(e => e.ShortDeliveryNotes).HasMaxLength(1000);

        builder.HasIndex(e => e.TransportRequestId).IsUnique();
    }
}

public class TransportDocumentConfiguration : BaseEntityConfiguration<TransportDocument>
{
    protected override string EntityName => "TransportDocument";

    public override void Configure(EntityTypeBuilder<TransportDocument> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportDocuments");

        builder.Property(e => e.FileName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.FileUrl).HasMaxLength(500).IsRequired();
        builder.Property(e => e.ContentType).HasMaxLength(100);
        builder.Property(e => e.Description).HasMaxLength(500);

        builder.HasIndex(e => e.TransportRequestId);
    }
}

public class TransportExpenseConfiguration : BaseEntityConfiguration<TransportExpense>
{
    protected override string EntityName => "TransportExpense";

    public override void Configure(EntityTypeBuilder<TransportExpense> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportExpenses");

        builder.Property(e => e.CategoryDescription).HasMaxLength(200);
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(3).HasDefaultValue("INR");
        builder.Property(e => e.ExpenseDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.Remarks).HasMaxLength(500);
        builder.Property(e => e.ReceiptUrl).HasMaxLength(500);

        builder.HasIndex(e => e.TransportRequestId);
        builder.HasIndex(e => e.TransportVehicleId);
    }
}
