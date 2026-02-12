using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

public class TransporterConfiguration : BaseEntityConfiguration<Transporter>
{
    protected override string EntityName => "Transporter";

    public override void Configure(EntityTypeBuilder<Transporter> builder)
    {
        base.Configure(builder);
        builder.ToTable("Transporters");

        builder.Property(e => e.TransporterName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ContactPerson).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Email).HasMaxLength(200);
        builder.Property(e => e.PANNumber).HasMaxLength(15);
        builder.Property(e => e.GSTNumber).HasMaxLength(20);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.State).HasMaxLength(100);
        builder.Property(e => e.Pincode).HasMaxLength(10);
        builder.Property(e => e.CountryCode).HasMaxLength(5);
        builder.Property(e => e.SuspensionReason).HasMaxLength(500);
        builder.Property(e => e.SuspensionDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.Rating).HasColumnType("decimal(3,1)").HasDefaultValue(0m);

        builder.HasIndex(e => e.TransporterName);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.BranchId);

        // Children
        builder.HasMany(e => e.KYCDocuments)
            .WithOne(k => k.Transporter)
            .HasForeignKey(k => k.TransporterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.BankAccounts)
            .WithOne(b => b.Transporter)
            .HasForeignKey(b => b.TransporterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.NotificationSettings)
            .WithOne(n => n.Transporter)
            .HasForeignKey(n => n.TransporterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class TransporterKYCConfiguration : BaseEntityConfiguration<TransporterKYC>
{
    protected override string EntityName => "TransporterKYC";

    public override void Configure(EntityTypeBuilder<TransporterKYC> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransporterKYCs");

        builder.Property(e => e.DocumentType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FileName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.FileUrl).HasMaxLength(500).IsRequired();
        builder.Property(e => e.ContentType).HasMaxLength(100);
        builder.Property(e => e.ExpiryDate).HasColumnType("datetime2(7)");
        builder.Property(e => e.VerifiedByName).HasMaxLength(100);
        builder.Property(e => e.VerifiedDate).HasColumnType("datetime2(7)");

        builder.HasIndex(e => e.TransporterId);
    }
}

public class TransporterBankConfiguration : BaseEntityConfiguration<TransporterBank>
{
    protected override string EntityName => "TransporterBank";

    public override void Configure(EntityTypeBuilder<TransporterBank> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransporterBanks");

        builder.Property(e => e.BankName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.AccountNumber).HasMaxLength(30).IsRequired();
        builder.Property(e => e.IFSCCode).HasMaxLength(15);
        builder.Property(e => e.BranchName).HasMaxLength(200);
        builder.Property(e => e.AccountHolderName).HasMaxLength(200);

        builder.HasIndex(e => e.TransporterId);
    }
}

public class TransporterNotificationConfiguration : BaseEntityConfiguration<TransporterNotification>
{
    protected override string EntityName => "TransporterNotification";

    public override void Configure(EntityTypeBuilder<TransporterNotification> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransporterNotifications");

        builder.Property(e => e.Destination).HasMaxLength(200);
        builder.Property(e => e.IsEnabled).HasDefaultValue(true);

        builder.HasIndex(e => e.TransporterId);
    }
}
