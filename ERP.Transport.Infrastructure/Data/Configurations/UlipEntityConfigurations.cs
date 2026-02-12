using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

// ── VehicleDetail ───────────────────────────────────────────────

public class VehicleDetailConfiguration : BaseEntityConfiguration<VehicleDetail>
{
    protected override string EntityName => "VehicleDetail";

    public override void Configure(EntityTypeBuilder<VehicleDetail> builder)
    {
        base.Configure(builder);
        builder.ToTable("VehicleDetails");

        builder.Property(e => e.VehicleNumber).HasMaxLength(20).IsRequired();
        builder.Property(e => e.OwnerName).HasMaxLength(200);
        builder.Property(e => e.FatherName).HasMaxLength(200);
        builder.Property(e => e.PresentAddress).HasMaxLength(500);
        builder.Property(e => e.PermanentAddress).HasMaxLength(500);
        builder.Property(e => e.VehicleClass).HasMaxLength(100);
        builder.Property(e => e.VehicleCategory).HasMaxLength(50);
        builder.Property(e => e.MakerModel).HasMaxLength(200);
        builder.Property(e => e.MakerDescription).HasMaxLength(200);
        builder.Property(e => e.BodyType).HasMaxLength(100);
        builder.Property(e => e.FuelType).HasMaxLength(50);
        builder.Property(e => e.Color).HasMaxLength(50);
        builder.Property(e => e.GrossVehicleWeight).HasColumnType("decimal(10,2)");
        builder.Property(e => e.UnladenWeight).HasColumnType("decimal(10,2)");
        builder.Property(e => e.EngineNumber).HasMaxLength(50);
        builder.Property(e => e.ChassisNumber).HasMaxLength(50);
        builder.Property(e => e.RegisteringAuthority).HasMaxLength(100);
        builder.Property(e => e.RegistrationState).HasMaxLength(50);
        builder.Property(e => e.RegistrationDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.FitnessUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.InsuranceUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.InsuranceCompany).HasMaxLength(200);
        builder.Property(e => e.InsurancePolicyNumber).HasMaxLength(100);
        builder.Property(e => e.PucValidUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.TaxValidUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.PermitValidUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.PermitType).HasMaxLength(100);
        builder.Property(e => e.NationalPermitNumber).HasMaxLength(50);
        builder.Property(e => e.NationalPermitUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.VehicleStatus).HasMaxLength(50);
        builder.Property(e => e.FinancerName).HasMaxLength(200);
        builder.Property(e => e.LastFetchedFromUlip).HasColumnType("datetime2(7)");

        // Unique index on VehicleNumber (one cached record per vehicle)
        builder.HasIndex(e => e.VehicleNumber).IsUnique();
    }
}

// ── DriverLicenseDetail ─────────────────────────────────────────

public class DriverLicenseDetailConfiguration : BaseEntityConfiguration<DriverLicenseDetail>
{
    protected override string EntityName => "DriverLicenseDetail";

    public override void Configure(EntityTypeBuilder<DriverLicenseDetail> builder)
    {
        base.Configure(builder);
        builder.ToTable("DriverLicenseDetails");

        builder.Property(e => e.LicenseNumber).HasMaxLength(30).IsRequired();
        builder.Property(e => e.DateOfBirth).HasColumnType("date");
        builder.Property(e => e.HolderName).HasMaxLength(200);
        builder.Property(e => e.FatherOrHusbandName).HasMaxLength(200);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.State).HasMaxLength(50);
        builder.Property(e => e.PinCode).HasMaxLength(10);
        builder.Property(e => e.BloodGroup).HasMaxLength(10);
        builder.Property(e => e.Gender).HasMaxLength(10);
        builder.Property(e => e.PhotoUrl).HasMaxLength(500);
        builder.Property(e => e.IssueDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.ValidFrom).HasColumnType("datetime2(7)");
        builder.Property(e => e.ValidTo).HasColumnType("datetime2(7)");
        builder.Property(e => e.IssuingAuthority).HasMaxLength(200);
        builder.Property(e => e.VehicleClassesAuthorized).HasMaxLength(200);
        builder.Property(e => e.LicenseStatus).HasMaxLength(50);
        builder.Property(e => e.LastFetchedFromUlip).HasColumnType("datetime2(7)");

        // Unique index on LicenseNumber (one cached record per license)
        builder.HasIndex(e => e.LicenseNumber).IsUnique();
    }
}

// ── FASTagTransaction ───────────────────────────────────────────

public class FASTagTransactionConfiguration : BaseEntityConfiguration<FASTagTransaction>
{
    protected override string EntityName => "FASTagTransaction";

    public override void Configure(EntityTypeBuilder<FASTagTransaction> builder)
    {
        base.Configure(builder);
        builder.ToTable("FASTagTransactions");

        builder.Property(e => e.VehicleNumber).HasMaxLength(20).IsRequired();
        builder.Property(e => e.TagId).HasMaxLength(50);
        builder.Property(e => e.TollPlazaName).HasMaxLength(200);
        builder.Property(e => e.TollPlazaId).HasMaxLength(50);
        builder.Property(e => e.LaneDirection).HasMaxLength(20);
        builder.Property(e => e.TransactionDateTime).HasColumnType("datetime2(7)");
        builder.Property(e => e.TransactionId).HasMaxLength(100);
        builder.Property(e => e.TransactionStatus).HasMaxLength(30);
        builder.Property(e => e.TransactionAmount).HasColumnType("decimal(12,2)");
        builder.Property(e => e.PlazaState).HasMaxLength(50);
        builder.Property(e => e.PlazaHighway).HasMaxLength(100);
        builder.Property(e => e.VehicleClassAtToll).HasMaxLength(50);
        builder.Property(e => e.FetchedFromUlip).HasColumnType("datetime2(7)");

        // FK to TransportRequest (optional)
        builder.HasOne(e => e.TransportRequest)
            .WithMany()
            .HasForeignKey(e => e.TransportRequestId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => e.VehicleNumber);
        builder.HasIndex(e => e.TransportRequestId);
        builder.HasIndex(e => e.TransactionDateTime);
    }
}

// ── TollPlaza ───────────────────────────────────────────────────

public class TollPlazaConfiguration : BaseEntityConfiguration<TollPlaza>
{
    protected override string EntityName => "TollPlaza";

    public override void Configure(EntityTypeBuilder<TollPlaza> builder)
    {
        base.Configure(builder);
        builder.ToTable("TollPlazas");

        builder.Property(e => e.PlazaCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PlazaName).HasMaxLength(200);
        builder.Property(e => e.State).HasMaxLength(50);
        builder.Property(e => e.Highway).HasMaxLength(100);
        builder.Property(e => e.Direction).HasMaxLength(50);
        builder.Property(e => e.SingleJourneyRate).HasColumnType("decimal(10,2)");
        builder.Property(e => e.ReturnJourneyRate).HasColumnType("decimal(10,2)");
        builder.Property(e => e.MonthlyPassRate).HasColumnType("decimal(10,2)");
        builder.Property(e => e.VehicleClassApplicable).HasMaxLength(100);
        builder.Property(e => e.LastFetchedFromUlip).HasColumnType("datetime2(7)");

        builder.HasIndex(e => e.PlazaCode).IsUnique();
    }
}

// ── EWayBill ────────────────────────────────────────────────────

public class EWayBillConfiguration : BaseEntityConfiguration<EWayBill>
{
    protected override string EntityName => "EWayBill";

    public override void Configure(EntityTypeBuilder<EWayBill> builder)
    {
        base.Configure(builder);
        builder.ToTable("EWayBills");

        builder.Property(e => e.EWayBillNumber).HasMaxLength(50);
        builder.Property(e => e.GeneratedDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.ValidUpto).HasColumnType("datetime2(7)");
        builder.Property(e => e.EWayBillStatus).HasMaxLength(30);
        builder.Property(e => e.SupplierGstin).HasMaxLength(20);
        builder.Property(e => e.SupplierName).HasMaxLength(200);
        builder.Property(e => e.RecipientGstin).HasMaxLength(20);
        builder.Property(e => e.RecipientName).HasMaxLength(200);
        builder.Property(e => e.DocumentType).HasMaxLength(30);
        builder.Property(e => e.DocumentNumber).HasMaxLength(50);
        builder.Property(e => e.DocumentDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.HsnCode).HasMaxLength(20);
        builder.Property(e => e.ProductDescription).HasMaxLength(500);
        builder.Property(e => e.TaxableAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CgstAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.SgstAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.IgstAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalInvoiceValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TransporterGstin).HasMaxLength(20);
        builder.Property(e => e.TransporterName).HasMaxLength(200);
        builder.Property(e => e.TransportMode).HasMaxLength(20);
        builder.Property(e => e.VehicleNumber).HasMaxLength(20);
        builder.Property(e => e.VehicleType).HasMaxLength(50);
        builder.Property(e => e.ApproximateDistanceKm).HasColumnType("decimal(10,2)");
        builder.Property(e => e.FromState).HasMaxLength(50);
        builder.Property(e => e.FromPlace).HasMaxLength(200);
        builder.Property(e => e.FromPincode).HasMaxLength(10);
        builder.Property(e => e.ToState).HasMaxLength(50);
        builder.Property(e => e.ToPlace).HasMaxLength(200);
        builder.Property(e => e.ToPincode).HasMaxLength(10);
        builder.Property(e => e.LastFetchedFromApi).HasColumnType("datetime2(7)");

        // FK to TransportRequest
        builder.HasOne(e => e.TransportRequest)
            .WithMany()
            .HasForeignKey(e => e.TransportRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.EWayBillNumber);
        builder.HasIndex(e => e.TransportRequestId);
    }
}
