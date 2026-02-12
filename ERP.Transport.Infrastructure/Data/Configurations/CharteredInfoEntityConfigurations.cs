using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

// ── EInvoice ────────────────────────────────────────────────────

public class EInvoiceConfiguration : BaseEntityConfiguration<EInvoice>
{
    protected override string EntityName => "EInvoice";

    public override void Configure(EntityTypeBuilder<EInvoice> builder)
    {
        base.Configure(builder);
        builder.ToTable("EInvoices");

        builder.Property(e => e.Irn).HasMaxLength(100);
        builder.Property(e => e.AckNumber).HasMaxLength(50);
        builder.Property(e => e.AckDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.EInvoiceStatus).HasMaxLength(30);
        builder.Property(e => e.DocumentType).HasMaxLength(10);
        builder.Property(e => e.DocumentNumber).HasMaxLength(50);
        builder.Property(e => e.DocumentDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.SellerGstin).HasMaxLength(20);
        builder.Property(e => e.SellerLegalName).HasMaxLength(200);
        builder.Property(e => e.SellerTradeName).HasMaxLength(200);
        builder.Property(e => e.BuyerGstin).HasMaxLength(20);
        builder.Property(e => e.BuyerLegalName).HasMaxLength(200);
        builder.Property(e => e.BuyerTradeName).HasMaxLength(200);
        builder.Property(e => e.TotalAssessableValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalCgstValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalSgstValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalIgstValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalCessValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalInvoiceValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.EwbDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.EwbValidTill).HasColumnType("datetime2(7)");
        builder.Property(e => e.CancelReason).HasMaxLength(50);
        builder.Property(e => e.CancelRemarks).HasMaxLength(500);
        builder.Property(e => e.CancelledDate).HasColumnType("datetime2(7)");

        // FK to TransportRequest (optional)
        builder.HasOne(e => e.TransportRequest)
            .WithMany()
            .HasForeignKey(e => e.TransportRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.Irn);
        builder.HasIndex(e => e.TransportRequestId);
        builder.HasIndex(e => e.DocumentNumber);
        builder.HasIndex(e => e.EwbNumber);
    }
}

// ── GstDetail ───────────────────────────────────────────────────

public class GstDetailConfiguration : BaseEntityConfiguration<GstDetail>
{
    protected override string EntityName => "GstDetail";

    public override void Configure(EntityTypeBuilder<GstDetail> builder)
    {
        base.Configure(builder);
        builder.ToTable("GstDetails");

        builder.Property(e => e.Gstin).HasMaxLength(20).IsRequired();
        builder.Property(e => e.LegalName).HasMaxLength(200);
        builder.Property(e => e.TradeName).HasMaxLength(200);
        builder.Property(e => e.BusinessType).HasMaxLength(100);
        builder.Property(e => e.GstinStatus).HasMaxLength(30);
        builder.Property(e => e.RegistrationDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.CancellationDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.StateCode).HasMaxLength(5);
        builder.Property(e => e.StateName).HasMaxLength(50);
        builder.Property(e => e.Pincode).HasMaxLength(10);
        builder.Property(e => e.NatureOfBusiness).HasMaxLength(200);
        builder.Property(e => e.ConstitutionOfBusiness).HasMaxLength(200);
        builder.Property(e => e.TaxpayerType).HasMaxLength(50);
        builder.Property(e => e.LastFiledReturn).HasMaxLength(50);
        builder.Property(e => e.LastFiledReturnDate).HasMaxLength(20);
        builder.Property(e => e.LastFetchedFromApi).HasColumnType("datetime2(7)");

        // Unique index on GSTIN
        builder.HasIndex(e => e.Gstin).IsUnique();
    }
}
