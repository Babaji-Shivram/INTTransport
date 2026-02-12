using ERP.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Transport.Infrastructure.Data.Configurations;

/// <summary>
/// EF configuration for the TransportLookup master/code table.
/// </summary>
public class TransportLookupConfiguration : BaseEntityConfiguration<TransportLookup>
{
    protected override string EntityName => "TransportLookup";

    public override void Configure(EntityTypeBuilder<TransportLookup> builder)
    {
        base.Configure(builder);
        builder.ToTable("TransportLookups");

        builder.Property(e => e.Code).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.CountryCode).HasMaxLength(10);

        builder.HasIndex(e => new { e.Category, e.Code }).IsUnique();
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => e.IsActive);
    }
}
